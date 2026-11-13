/*
 * Calendar Management System - Settings Page
 * Next.js 16 App Router + TypeScript + Tailwind CSS
 */

"use client"

import { useState, useEffect, useCallback } from "react"
import { useRouter } from "next/navigation"
import { useTheme } from "next-themes"
import { toast } from "sonner"
import {
  Sidebar,
} from "@/app/components/features/sidebar/Sidebar"
import {
  Button,
} from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import { Skeleton } from "@/components/ui/skeleton"
import {
  ToggleGroup,
  ToggleGroupItem,
} from "@/components/ui/toggle-group"
import {
  ChevronLeft,
  X,
  Menu,
  User as UserIcon,
  Mail as MailIcon,
  Bell as BellIcon,
  Shield as ShieldIcon,
  Globe as GlobeIcon,
  Trash2,
  RefreshCw,
  Sun,
  Moon,
  Monitor,
} from "lucide-react"
import { apiService } from "@/components/services/api"
import type {
  UserProfileDto,
  UserSettingsDto,
} from "@/types/calendar"

export default function SettingsPage() {
  const router = useRouter()
  const { theme: currentTheme, setTheme } = useTheme()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  // Dialog states
  const [editProfileOpen, setEditProfileOpen] = useState(false)
  const [changeEmailOpen, setChangeEmailOpen] = useState(false)
  const [changePasswordOpen, setChangePasswordOpen] = useState(false)
  const [deleteAccountOpen, setDeleteAccountOpen] = useState(false)

  // Loading states
  const [isLoading, setIsLoading] = useState(false)
  const [isSaving, setIsSaving] = useState(false)

  // State
  const [profile, setProfile] = useState<UserProfileDto | null>(null)
  const [settings, setSettings] = useState<UserSettingsDto | null>(null)

  // Form states
  const [firstName, setFirstName] = useState("")
  const [lastName, setLastName] = useState("")
  const [newEmail, setNewEmail] = useState("")
  const [currentPassword, setCurrentPassword] = useState("")
  const [newPassword, setNewPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")

  // Error state
  const [error, setError] = useState<string | null>(null)

  // Clear error
  const clearError = () => {
    setError(null)
  }

  // Load data
  const loadData = useCallback(async () => {
    setIsLoading(true)
    setError(null)
    try {
      const [profile, settings] = await Promise.all([
        apiService.getUserProfile(),
        apiService.getUserSettings(),
      ])
      setProfile(profile)
      setSettings(settings)
      setFirstName(profile.firstName)
      setLastName(profile.lastName)
      setNewEmail(profile.email)
    } catch (err: any) {
      setError(err.detail || err.message || "Failed to load settings. Please check your connection.")
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    loadData()
  }, [loadData])

  const applyTheme = (theme: string) => {
    if (theme === "dark") {
      document.documentElement.classList.add("dark")
    } else {
      document.documentElement.classList.remove("dark")
    }
  }

  // Apply theme on load
  useEffect(() => {
    if (settings) {
      applyTheme(settings.theme)
    }
  }, [settings])

  const handleEditProfile = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSaving(true)
    try {
      await apiService.updateProfile({
        firstName,
        lastName,
      })
      toast.success("Profile updated successfully")
      await loadData()
      setEditProfileOpen(false)
    } catch (err: any) {
      toast.error(err.detail || "Failed to update profile")
    } finally {
      setIsSaving(false)
    }
  }

  const handleChangeEmail = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSaving(true)
    try {
      await apiService.updateProfile({
        firstName: profile?.firstName || "",
        lastName: profile?.lastName || "",
      })
      toast.success("Email updated successfully")
      await loadData()
      setChangeEmailOpen(false)
    } catch (err: any) {
      toast.error(err.detail || "Failed to update email")
    } finally {
      setIsSaving(false)
    }
  }

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSaving(true)
    try {
      if (newPassword.length < 8) {
        toast.error("Password must be at least 8 characters")
        setIsSaving(false)
        return
      }
      if (!/[A-Z]/.test(newPassword)) {
        toast.error("Password must contain at least one uppercase letter")
        setIsSaving(false)
        return
      }
      if (!/[0-9]/.test(newPassword)) {
        toast.error("Password must contain at least one number")
        setIsSaving(false)
        return
      }
      if (newPassword !== confirmPassword) {
        toast.error("Passwords do not match")
        setIsSaving(false)
        return
      }

      await apiService.changePassword({
        currentPassword,
        newPassword,
        confirmPassword,
      })
      toast.success("Password changed successfully")
      setChangePasswordOpen(false)
      setCurrentPassword("")
      setNewPassword("")
      setConfirmPassword("")
    } catch (err: any) {
      toast.error(err.detail || "Failed to change password")
    } finally {
      setIsSaving(false)
    }
  }

  const handleDeleteAccount = async () => {
    setIsSaving(true)
    try {
      await apiService.deleteAccount()
      toast.success("Account deleted successfully")
      setTimeout(() => {
        router.push("/")
      }, 1500)
    } catch (err: any) {
      toast.error(err.detail || "Failed to delete account")
    } finally {
      setIsSaving(false)
      setDeleteAccountOpen(false)
    }
  }

  const toggleNotifications = async () => {
    if (!settings) return
    const newValue = !settings.notificationsEnabled
    // Optimistic update
    setSettings((prev) => ({ ...prev!, notificationsEnabled: newValue }))
    try {
      await apiService.updateSettings({
        theme: settings.theme,
        notificationsEnabled: newValue,
        emailRemindersEnabled: settings.emailRemindersEnabled,
      })
    } catch (err: any) {
      // Rollback
      setSettings((prev) => ({ ...prev!, notificationsEnabled: !newValue }))
      toast.error(err.detail || "Failed to update notification settings")
    }
  }

  const toggleEmailReminders = async () => {
    if (!settings) return
    const newValue = !settings.emailRemindersEnabled
    // Optimistic update
    setSettings((prev) => ({ ...prev!, emailRemindersEnabled: newValue }))
    try {
      await apiService.updateSettings({
        theme: settings.theme,
        notificationsEnabled: settings.notificationsEnabled,
        emailRemindersEnabled: newValue,
      })
    } catch (err: any) {
      // Rollback
      setSettings((prev) => ({ ...prev!, emailRemindersEnabled: !newValue }))
      toast.error(err.detail || "Failed to update reminder settings")
    }
  }

  const handleThemeChange = async (theme: string) => {
    if (!settings) return
    const previousTheme = settings.theme
    // Optimistic update
    setSettings((prev) => ({ ...prev!, theme }))
    setTheme(theme)
    try {
      await apiService.updateSettings({
        theme,
        notificationsEnabled: settings.notificationsEnabled,
        emailRemindersEnabled: settings.emailRemindersEnabled,
      })
      toast.success("Theme updated successfully")
    } catch (err: any) {
      // Rollback
      setSettings((prev) => ({ ...prev!, theme: previousTheme }))
      setTheme(previousTheme)
      toast.error(err.detail || "Failed to update theme")
    }
  }

  return (
    <div className="min-h-screen bg-muted/40 flex flex-col">
      {/* Top Navigation */}
      <header className="sticky top-0 z-30 bg-card border-b border-border">
        <div className="flex items-center justify-between px-6 py-3 bg-card">
          <div className="flex items-center gap-2">
            <button
              onClick={() => setSidebarOpen(!sidebarOpen)}
              className="rounded-lg p-2 text-muted-foreground hover:bg-accent hover:text-accent-foreground md:hidden cursor-pointer"
              aria-label="Toggle sidebar"
            >
              <Menu className="h-4 w-4" />
            </button>
            <div className="flex items-center gap-2">
              <div className="w-9 h-9 rounded-lg bg-primary text-primary-foreground flex items-center justify-center font-bold">
                S
              </div>
              <h1 className="font-semibold text-lg tracking-tight">Settings</h1>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <Button variant="outline" size="sm" onClick={loadData} disabled={isLoading} className="cursor-pointer">
              <RefreshCw className={isLoading ? "h-4 w-4 animate-spin mr-2" : "h-4 w-4 mr-2"} />
              Refresh
            </Button>
          </div>
        </div>
      </header>

      <div className="flex flex-1 min-h-0">
        {/* Desktop Sidebar */}
        <Sidebar
          isOpen={sidebarOpen}
          onClose={() => setSidebarOpen(false)}
        />

        {/* Mobile Sidebar Overlay */}
        {sidebarOpen && (
          <div
            className="fixed inset-0 z-40 bg-black/80 backdrop-blur-sm md:hidden"
            onClick={() => setSidebarOpen(false)}
          />
        )}

        {/* Main Content */}
        <main className="flex-1 min-w-0 overflow-y-auto p-4 md:p-6">
          <div className="mx-auto max-w-[1200px] px-6 py-8">
            <div className="space-y-6">
              <div>
                <h2 className="text-2xl font-semibold tracking-tight text-foreground">Settings</h2>
                <p className="text-sm text-muted-foreground mt-1">Manage your account, security and application preferences.</p>
              </div>

              {error && (
                <div className="rounded-xl bg-red-50 p-4 border border-red-100">
                  <h3 className="text-sm font-semibold text-red-800">Error</h3>
                  <p className="text-sm text-red-600 mt-1">{error}</p>
                  <Button onClick={loadData} size="sm" variant="outline" className="mt-3 cursor-pointer">
                    <RefreshCw className="h-4 w-4 mr-2" />
                    Retry
                  </Button>
                </div>
              )}

              {isLoading && !profile && (
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  <Skeleton className="h-48" />
                  <Skeleton className="h-48" />
                  <Skeleton className="h-48" />
                  <Skeleton className="h-48" />
                  <Skeleton className="h-32 lg:col-span-2" />
                </div>
              )}

              {profile && settings && (
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  {/* Account Settings */}
                  <Card className="bg-card border rounded-xl shadow-sm p-6">
                    <CardHeader>
                      <CardTitle className="text-base font-semibold">Account</CardTitle>
                      <CardDescription>Manage your account information</CardDescription>
                    </CardHeader>
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between gap-4 py-4 border-b border-border">
                        <div className="flex items-start gap-3">
                          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-blue-50 text-blue-600 dark:bg-blue-950">
                            <UserIcon className="h-[18px] w-[18px]" />
                          </div>
                          <div>
                            <p className="text-sm font-medium">Profile</p>
                            <p className="text-sm text-muted-foreground">{profile.firstName} {profile.lastName}</p>
                          </div>
                        </div>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => setEditProfileOpen(true)}
                          className="cursor-pointer"
                        >
                          Edit Profile
                        </Button>
                      </div>
                      <div className="flex items-center justify-between gap-4 py-4">
                        <div className="flex items-start gap-3">
                          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-blue-50 text-blue-600 dark:bg-blue-950">
                            <MailIcon className="h-[18px] w-[18px]" />
                          </div>
                          <div>
                            <p className="text-sm font-medium">Email Address</p>
                            <p className="text-sm text-muted-foreground">{profile.email}</p>
                          </div>
                        </div>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => {
                            setNewEmail(profile.email)
                            setChangeEmailOpen(true)
                          }}
                          className="cursor-pointer"
                        >
                          Change Email
                        </Button>
                      </div>
                    </CardContent>
                  </Card>

                  {/* Notification Preferences */}
                  <Card className="bg-card border rounded-xl shadow-sm p-6">
                    <CardHeader>
                      <CardTitle className="text-base font-semibold">Notifications</CardTitle>
                      <CardDescription>Manage notification preferences</CardDescription>
                    </CardHeader>
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between gap-4 py-4 border-b border-border">
                        <div className="flex items-start gap-3">
                          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-blue-50 text-blue-600 dark:bg-blue-950">
                            <BellIcon className="h-[18px] w-[18px]" />
                          </div>
                          <div>
                            <p className="text-sm font-medium">Application Notifications</p>
                            <p className="text-sm text-muted-foreground">Receive meeting notifications</p>
                          </div>
                        </div>
                        <Switch
                          checked={settings.notificationsEnabled}
                          onCheckedChange={toggleNotifications}
                          disabled={isSaving}
                          className="cursor-pointer"
                        />
                      </div>
                      <div className="flex items-center justify-between gap-4 py-4">
                        <div className="flex items-start gap-3">
                          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-blue-50 text-blue-600 dark:bg-blue-950">
                            <MailIcon className="h-[18px] w-[18px]" />
                          </div>
                          <div>
                            <p className="text-sm font-medium">Email Reminders</p>
                            <p className="text-sm text-muted-foreground">Get notified about upcoming meetings</p>
                          </div>
                        </div>
                        <Switch
                          checked={settings.emailRemindersEnabled}
                          onCheckedChange={toggleEmailReminders}
                          disabled={isSaving}
                          className="cursor-pointer"
                        />
                      </div>
                    </CardContent>
                  </Card>

                  {/* Security */}
                  <Card className="bg-card border rounded-xl shadow-sm p-6">
                    <CardHeader>
                      <CardTitle className="text-base font-semibold">Security</CardTitle>
                      <CardDescription>Manage your password and security</CardDescription>
                    </CardHeader>
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between gap-4 py-4">
                        <div className="flex items-start gap-3">
                          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-blue-50 text-blue-600 dark:bg-blue-950">
                            <ShieldIcon className="h-[18px] w-[18px]" />
                          </div>
                          <div>
                            <p className="text-sm font-medium">Password</p>
                            <p className="text-sm text-muted-foreground">Change your password</p>
                          </div>
                        </div>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => setChangePasswordOpen(true)}
                          className="cursor-pointer"
                        >
                          Change
                        </Button>
                      </div>
                    </CardContent>
                  </Card>

                  {/* Appearance */}
                  <Card className="bg-card border rounded-xl shadow-sm p-6">
                    <CardHeader>
                      <CardTitle className="text-base font-semibold">Appearance</CardTitle>
                      <CardDescription>Customize your experience</CardDescription>
                    </CardHeader>
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between gap-4 py-4">
                        <div className="flex items-start gap-3">
                          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-blue-50 text-blue-600 dark:bg-blue-950">
                            <GlobeIcon className="h-[18px] w-[18px]" />
                          </div>
                          <div>
                            <p className="text-sm font-medium">Theme</p>
                            <p className="text-sm text-muted-foreground">Choose your theme</p>
                          </div>
                        </div>
                        <ToggleGroup
                          type="single"
                          value={settings.theme}
                          onValueChange={(value) => {
                            if (value) handleThemeChange(value)
                          }}
                          className="cursor-pointer"
                        >
                          <ToggleGroupItem value="light" aria-label="Light theme" className="cursor-pointer">
                            <Sun className="h-4 w-4 mr-1.5" />
                            Light
                          </ToggleGroupItem>
                          <ToggleGroupItem value="dark" aria-label="Dark theme" className="cursor-pointer">
                            <Moon className="h-4 w-4 mr-1.5" />
                            Dark
                          </ToggleGroupItem>
                          <ToggleGroupItem value="system" aria-label="System theme" className="cursor-pointer">
                            <Monitor className="h-4 w-4 mr-1.5" />
                            System
                          </ToggleGroupItem>
                        </ToggleGroup>
                      </div>
                    </CardContent>
                  </Card>

                  {/* Danger Zone - Full Width */}
                  <Card className="lg:col-span-2 bg-card border border-destructive/30 rounded-xl shadow-sm p-6">
                    <CardHeader>
                      <CardTitle className="text-base font-semibold text-destructive">Danger Zone</CardTitle>
                      <CardDescription>Irreversible actions</CardDescription>
                    </CardHeader>
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between gap-4 py-4">
                        <div className="flex items-start gap-3">
                          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-red-50 text-red-600">
                            <Trash2 className="h-[18px] w-[18px]" />
                          </div>
                          <div>
                            <p className="text-sm font-medium">Delete Account</p>
                            <p className="text-sm text-muted-foreground">Permanently delete your account and all data</p>
                          </div>
                        </div>
                        <AlertDialog open={deleteAccountOpen} onOpenChange={setDeleteAccountOpen}>
                          <AlertDialogTrigger asChild>
                            <Button
                              variant="outline"
                              size="sm"
                              className="cursor-pointer text-destructive border-destructive hover:bg-destructive/10"
                            >
                              Delete Account
                            </Button>
                          </AlertDialogTrigger>
                          <AlertDialogContent>
                            <AlertDialogHeader>
                              <AlertDialogTitle>Delete Account</AlertDialogTitle>
                              <AlertDialogDescription>
                                Are you sure you want to delete your account? This action cannot be undone.
                                All your events, meetings, and data will be permanently removed.
                              </AlertDialogDescription>
                            </AlertDialogHeader>
                            <AlertDialogFooter>
                              <AlertDialogCancel className="cursor-pointer">
                                Cancel
                              </AlertDialogCancel>
                              <AlertDialogAction
                                onClick={handleDeleteAccount}
                                disabled={isSaving}
                                className="cursor-pointer bg-destructive text-destructive-foreground hover:bg-destructive/90"
                              >
                                {isSaving ? "Deleting..." : "Delete Account"}
                              </AlertDialogAction>
                            </AlertDialogFooter>
                          </AlertDialogContent>
                        </AlertDialog>
                      </div>
                    </CardContent>
                  </Card>
                </div>
              )}
            </div>
          </div>
        </main>
      </div>

      {/* Edit Profile Dialog */}
      <Dialog open={editProfileOpen} onOpenChange={setEditProfileOpen}>
        <DialogContent className="sm:max-w-lg max-w-md w-full overflow-y-auto max-h-[90vh]">
          <DialogHeader>
            <DialogTitle>Edit Profile</DialogTitle>
            <DialogDescription>Update your profile information</DialogDescription>
          </DialogHeader>
          <form onSubmit={handleEditProfile} className="space-y-5 pt-2">
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="firstName">First Name</Label>
                <Input
                  id="firstName"
                  required
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  placeholder="First name"
                  className="border-input"
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="lastName">Last Name</Label>
                <Input
                  id="lastName"
                  required
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  placeholder="Last name"
                  className="border-input"
                />
              </div>
            </div>
            <div className="flex flex-row-reverse gap-3 mt-8">
              <Button
                type="button"
                variant="outline"
                onClick={() => setEditProfileOpen(false)}
                disabled={isSaving}
                className="w-auto cursor-pointer"
              >
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isSaving}
                className="w-auto cursor-pointer"
              >
                {isSaving ? "Saving..." : "Save Changes"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      {/* Change Email Dialog */}
      <Dialog open={changeEmailOpen} onOpenChange={setChangeEmailOpen}>
        <DialogContent className="sm:max-w-lg max-w-md w-full overflow-y-auto max-h-[90vh]">
          <DialogHeader>
            <DialogTitle>Change Email</DialogTitle>
            <DialogDescription>Update your email address</DialogDescription>
          </DialogHeader>
          <form onSubmit={handleChangeEmail} className="space-y-5 pt-2">
            <div className="space-y-2">
              <Label htmlFor="newEmail">New Email Address</Label>
              <Input
                id="newEmail"
                type="email"
                required
                value={newEmail}
                onChange={(e) => setNewEmail(e.target.value)}
                placeholder="you@example.com"
                className="border-input"
              />
            </div>
            <div className="flex flex-row-reverse gap-3 mt-8">
              <Button
                type="button"
                variant="outline"
                onClick={() => setChangeEmailOpen(false)}
                disabled={isSaving}
                className="w-auto cursor-pointer"
              >
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isSaving}
                className="w-auto cursor-pointer"
              >
                {isSaving ? "Saving..." : "Change Email"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      {/* Change Password Dialog */}
      <Dialog open={changePasswordOpen} onOpenChange={setChangePasswordOpen}>
        <DialogContent className="sm:max-w-lg max-w-md w-full overflow-y-auto max-h-[90vh]">
          <DialogHeader>
            <DialogTitle>Change Password</DialogTitle>
            <DialogDescription>Update your password</DialogDescription>
          </DialogHeader>
          <form onSubmit={handleChangePassword} className="space-y-5 pt-2">
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="currentPassword">Current Password</Label>
                <Input
                  id="currentPassword"
                  type="password"
                  required
                  value={currentPassword}
                  onChange={(e) => setCurrentPassword(e.target.value)}
                  placeholder="Current password"
                  className="border-input"
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="newPassword">New Password</Label>
                <Input
                  id="newPassword"
                  type="password"
                  required
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  placeholder="New password (min 8 chars)"
                  className="border-input"
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="confirmPassword">Confirm Password</Label>
                <Input
                  id="confirmPassword"
                  type="password"
                  required
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  placeholder="Confirm new password"
                  className="border-input"
                />
              </div>
            </div>
            <div className="flex flex-row-reverse gap-3 mt-8">
              <Button
                type="button"
                variant="outline"
                onClick={() => setChangePasswordOpen(false)}
                disabled={isSaving}
                className="w-auto cursor-pointer"
              >
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isSaving}
                className="w-auto cursor-pointer"
              >
                {isSaving ? "Changing..." : "Change Password"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  )
}
