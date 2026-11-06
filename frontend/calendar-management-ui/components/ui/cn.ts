/**
 * Calendar Management System - UI Utility
 * cn() utility for merging Tailwind classes (Shadcn pattern)
 */

import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
