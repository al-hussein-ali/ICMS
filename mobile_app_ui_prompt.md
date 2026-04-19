# Mobile App UI Prompt – ICMS Offline Field Visit Operations App

This document serves as a comprehensive system prompt for any AI UI/UX generator (e.g., v0.dev, UIzard) or AI coding assistant (like Flutter/React Native code generators). Copy the text below to generate the exact UI screens required for the system.

---
**Copy the text below:**
---

Please act as an Expert Mobile App UI/UX Designer and Frontend Engineer. Generate high-fidelity, modern, offline-first mobile app screens and components for a "Field Visit Tracker" used by authorized medical staff. The code you generate (Flutter Widgets / React Native / React TS) must be production-ready, beautiful, and highly responsive.

## 1. App Context & Core Behaviors
*   **The Problem:** Workers visit remote areas to administer vaccinations. Internet connectivity is highly unreliable.
*   **The Paradigm:** The app is **Offline-First**. Any action the user takes is saved locally immediately. When the connection is restored, the app bulk-syncs data to the server.
*   **Worker Freedom:** Admins do *not* assign visits. Instead, any authorized worker can browse a pool of "Open Visits", select one, download the target lists, and start contributing offline.
*   **Core UI Feedback:** Every screen should gracefully handle being offline without throwing errors. The user must always know if they are working offline and how many items are waiting to be synced.

## 2. Design Aesthetic & The "Premium" Vibe
The app must not look generic. It requires a modern, smooth, and energetic design.

### Color Palette (Light / Dark Mode Support)
*   **Primary Brand:** `#2586C7` (Electric Blue) - Headers, Primary buttons.
*   **Secondary Accent:** `#02A3A9` (Teal) - Floating Action Buttons, active tabs.
*   **Backgrounds (Canvas):** `#F8FAFC` (Light Mode - Slate 50) / `#0F172A` (Dark Mode - Slate 900).
*   **Surface/Cards:** `#FFFFFF` (Light Mode) / `#1E293B` (Dark Mode).
*   **Status Indicators:** Success (`#10B981` Emerald), Warning (`#F59E0B` Amber), Danger/Reject (`#EF4444` Red).
*   **Gradients:** Use a subtle 135-degree gradient blending `#2586C7` to `#145E8F` for prominent elements like login buttons and hero banners.

### Typography & Structure
*   **Font:** Use a clean, modern san-serif (e.g., Inter, Outfit, or Poppins).
*   **Borders & Shadows:** Generous border-radius on cards (`12px` to `16px`). Soft, diffuse drop shadows rather than harsh dark shadows. Ensure clean spacing (padding inside cards).
*   **Micro-interactions:** Include skeleton loaders for fetching data, ripple effects on cards, and smooth bottom-sheet sliding animations. 

---

## 3. Required App Screens & Components

### A. The Global AppBar / Persistent Offline Header
*   Across most screens (except Login), the top app bar must include a **Sync Status Badge**.
*   **States:** "Online", "Offline (Working Locally)", "Syncing...", and "X Pending Items".
*   If there are Rejected Items, show a small red warning icon in the App Bar.

### B. Login & Splash Screen
*   Clean, centered form: Username, Password. 
*   **Action Button:** Full-width gradient Login button.
*   **Visual Check:** Show an animated spinner during auth. Show an offline alert if logging in without prior cached credentials while offline.

### C. Worker Dashboard (Home Screen)
*   **Hero Section:** A card displaying quick stats: "Completed Today: 45", "Pending Sync: 12".
*   **Main Navigation Cards (Grid or Large List):**
    1.  **Browse Open Campaigns:** Arrow icon, blue tint.
    2.  **Continue Active Visit:** Teal tint (Shows current downloaded visit, if any).
    3.  **Sync Center:** Amber tint if items pending or red if rejected data requires attention.

### D. Browse Available Field Visits (The "Pool")
*   A searchable, scrollable list of all active field visits happening locally (since workers select their own work now).
*   **Filters:** Filter chips for `Directorate`, `Neighborhood`, and `Date`.
*   **Card UI:** Visit Name, Location tags, Progress bar, Start/End Dates.
*   **Action:** When a visit is clicked, show a bottom sheet with a prominent **"Download & Start Contributing"** button. This simulate fetching the local cache.

### E. Offline Workspace: Active Field Visit Details
*   This is the primary offline workspace for the worker.
*   **Sticky Header:** Visit Title, and location breadcrumbs.
*   **Horizontal Tabs:**
    1.  **Target List:** Individuals due for vaccines today. **Crucial Layout:** Group these cards by **Sub-Neighborhood** using collapsible headers (accordions).
    2.  **Missed Doses:** Individuals in the area who missed a past dose. Highlight these cards with an amber tint or warning icon.
    3.  **Newly Registered:** People the worker met on the field and added today.
*   **List Item Card Elements:** Name, Age, Target Dose (e.g., "OPV-1"), Status indicator (Pending vs. Done).
*   **Action:** Floating Action Button (FAB) -> "Add Unregistered Person".

### F. Quick Vaccination Form (For Existing Targets)
*   Triggered when a worker taps an individual on the Target or Missed lists. It should slide up gracefully from the bottom (Bottom Sheet Modal).
*   **Fields required:**
    *   Dose (Dropdown, prepopulated with expected dose).
    *   Vaccination Date (Default to today).
    *   Note (Optional text area).
*   **Action:** Large "Save Offline" button with a success checkmark animation.

### G. Comprehensive Registration Form (The Ad-Hoc Form)
*   Triggered from the FAB. Used when encountering someone not on the Target List.
*   **Layout:** A smooth scrolling or multi-step wizard form. 
*   **Sections:**
    *   **Personal:** First, Second, Last Name, Gender, Date of Birth.
    *   **Contact:** Phone Number.
    *   **Location:** Linked Dropdowns for Directorate -> Neighborhood -> Sub-Neighborhood. (Must support offline selection).
    *   **Clinical:** Administered Dose, Date.
*   **Action:** "Save New Record locally".

### H. Sync Center & Conflict Resolution Screen
*   Dedicated screen for managing data traffic between local and server.
*   **Two Tabs:**
    1.  **Pending Queue:** List of operations waiting for internet (e.g., "Registered Dose for Ali", "Added New Person: Sara"). Shows a "Sync All Now" button at the bottom.
    2.  **Rejected Records:** Items the server rejected during a bulk sync (e.g., Duplicates, Validation fail).
*   **Conflict Card UI:** Shows the person's name, the field visit name, and a prominent red box explaining the **Rejection Reason** from the backend. Include a "Tap to Edit" button opening the form again to fix errors, and a "Delete" button.

---
**Instructions for the AI:** 
Please begin by generating the generic App Shell (Navigation, Offline Status App Bar), followed by the Dashboard, Active Field Visit view with its sub-tabs, and then the sync conflict resolution screen. Provide extensive layout code (State management hooks/setup is secondary to having the component structure, styling, and styling tokens correct). Use mock data showing Sub-Neighborhood groupings.
