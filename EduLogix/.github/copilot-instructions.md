# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction

## Code Style
- Use specific formatting rules
- Follow naming conventions
- Ensure security form class names are unique to avoid naming collisions with registrar forms (e.g., avoid `DashboardForm`/`AttendanceForm` overlaps).

## Project-Specific Rules
- DataGridView double-click events should be wired in the Designer file, not programmatically in the code-behind.
- The form should have fixed row/column sizes with no resizing capability.
- Limit changes to the `Attendance`, `Admin Dashboard`, `StudentInfo`, and `RegistrarStudAdd` only; do not modify `SecurityAttendance` for now.