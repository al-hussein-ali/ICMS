# Mobile App UI Prompt

This is a comprehensive prompt you can copy-paste to any AI UI/UX generator or AI coding assistant (like Flutter/React Native code generators) to generate screens that match your requirements.

---
**Copy the text below:**
---

Please generate the UI screens and components for an offline-first Mobile Field Visit App used by authorized medical staff (`FieldVisitWorker`). I need high-quality, modern, and highly responsive screens following a specific design language.

## Design Aesthetic & Color Palette
The app must have a modern, "Premium" feel using the following exact colors for both Light and Dark modes:

### Core Colors
*   **Brand Blue (Primary):** `#2586C7` - Use for branding, main buttons, and prominent headers.
*   **Teal/Cyan (Secondary):** `#02A3A9` - Use for secondary actions, accents, and active states.
*   **Navy Accent (Dark):** `#145E8F` - Use for dark backgrounds, footer elements, and deep gradients.

### Backgrounds & Surfaces
*   **Canvas BG (General Background):** `#F4F7F6` (Very light grey with a tint of green). In Dark Mode: `#121212`
*   **Surface/Card (White Background):** `#FFFFFF`. In Dark Mode: `#1E1E1E`

### Typography (Google Fonts like Inter or Roboto)
*   **Primary Text:** `#000000` (Light Mode) / `#FFFFFF` (Dark Mode)
*   **Muted Text:** `#666666` (Light Mode) / `#B3B3B3` (Dark Mode)

### Status Colors
*   **Success (Success/Done):** `#388E3C`
*   **Danger (Error/Delete/Rejected):** `#D32F2F`
*   **Warning (Alert/Pending):** `#F59E0B`

### Modern Gradients (Critical for Premium Look)
*   **Primary Gradient:** From `#2586C7` to `#145E8F` at a 135-degree angle.
*   **Secondary Gradient:** From `#02A3A9` to `#017A7E` at a 135-degree angle.

*Micro-animations, smooth transitions, and glassmorphism (where applicable without losing contrast) should be included.*

## Required Screens

### 1. Login Screen
*   A clean, centered form.
*   **Fields:** Username, Password.
*   **Action:** A beautifully styled "Login" button utilizing the **Primary Gradient**.
*   **State:** Show a spinner while authenticating.

### 2. Pending Field Visits Screen
*   A scrollable list of available, non-completed field visits assigned to the worker.
*   **Cards:** Each visit should be displayed as a card on the canvas background. Cards must have a slight shadow and border-radius.
*   **Action:** When clicked, the visit is selected, stored locally as the "Current Visit", and navigates to the Details Screen.

### 3. Field Visit Details Screen (Target List & Missed Doses)
*   **Header:** Shows the details of the selected Field Visit.
*   **Tabs / Sections:**
    1.  **Target Individuals:** List of individuals scheduled for this visit based on dates.
    2.  **Missed Doses:** List of sub-neighborhoods containing individuals who missed a dose.
*   **Floating Action Button (FAB) / Main Button:** "Add New Individual" (uses the **Secondary Gradient**).
*   **Offline Indicator:** A small UI element (e.g., cloud with slash) in the corner indicating that current operations are offline.

### 4. Register Dose Form (Existing Individual)
*   When a worker clicks on an individual in the Target List, this form slides up or opens.
*   **Fields (Matching `UpdateFieldVisitIndividualDto`):**
    *   Individual ID (Hidden/Read-only)
    *   Dose (Dropdown/Selection)
    *   Vaccination Date (Date Picker)
    *   Taken In (String/Dropdown)
    *   Note (Optional Text Area)
*   **Action:** "Save offline" button.

### 5. Add New Individual Form
*   Opened from the FAB in the Details Screen.
*   **Fields (Matching `NewFieldVaccinatedIndividualDto` and `PersonCreateDto`):**
    *   First Name, Second Name, Third Name (Optional), Last Name.
    *   Gender (Dropdown/Radio).
    *   Date of Birth (Date Picker).
    *   Phone Number.
    *   Directorate, Neighborhood, Sub-Neighborhood (Dropdowns).
    *   Dose (Dropdown).
    *   Vaccination Date (Date Picker).
    *   Taken In (Text).
    *   Note (Optional Text).
*   **Action:** "Save New Record" (Success colored button).

### 6. Rejected Records Screen
*   A dedicated warning-styled screen for data that failed synchronization logic on the server.
*   **List Items:** Show Individual's Full Name, the Field Visit ID/Name, and the **Rejection Reason**.
*   **Styling:** Make use of the **Danger** (`#D32F2F`) and **Warning** (`#F59E0B`) colors. Card borders could have a subtle red tint.

Please provide the layout and styling code (e.g., Widget trees for Flutter, or JSX/Tailwind for React Native) implementing these exact specifications.
