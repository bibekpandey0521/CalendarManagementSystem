"use client"

import * as React from "react"
import { cn } from "./cn"

interface BadgeProps
  extends React.HTMLAttributes<HTMLDivElement> {
  variant?: "default" | "secondary" | "destructive" | "outline"
}

const Badge = React.forwardRef<
  HTMLDivElement,
  BadgeProps
>(({ className, variant, ...props }, ref) => {
  return (
    <div
      ref={ref}
      className={cn(
        "inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2",
        variants[variant ?? "default"],
        className
      )}
      {...props}
    />
  )
})
Badge.displayName = "Badge"

const variants = {
  default: "bg-primary text-primary-foreground",
  secondary: "bg-secondary text-secondary-foreground",
  destructive:
    "bg-destructive text-destructive-foreground border-destructive/50",
  outline: "text-input border-input",
}

export { Badge }
