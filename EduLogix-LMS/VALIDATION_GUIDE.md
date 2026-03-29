# Input Validation & Kiosk Mode Implementation Guide

## Features Implemented

### 1. Input Validation Utility (`InputValidator.cs`)
A static utility class that provides comprehensive input validation methods for textboxes.

#### Available Validation Methods:

**Basic Validators:**
- `IsNotEmpty(textbox, fieldName)` - Checks if textbox is not empty
- `HasMinimumLength(textbox, minLength, fieldName)` - Validates minimum length
- `HasMaximumLength(textbox, maxLength, fieldName)` - Validates maximum length
- `ClearTextboxes(container)` - Clears all textboxes in a container

**Format Validators:**
- `IsNumericOnly(textbox, fieldName)` - Only allows numbers
- `IsAlphabeticOnly(textbox, fieldName)` - Only allows letters and spaces
- `IsAlphanumericOnly(textbox, fieldName)` - Only allows letters and numbers
- `IsValidEmail(textbox, fieldName)` - Validates email format
- `IsStrongPassword(textbox, fieldName)` - Validates password strength (min 8 chars, uppercase, lowercase, number)

---

### 2. Login Form (`Login.cs`)
Enhanced with kiosk mode functionality and input validation.

#### New Features:
- **Kiosk Mode Button Click Event**: `btnKioskMode_Click()` 
  - Opens the KioskForm when clicked
  - Hides the login form and shows it again when kiosk closes
  
- **Input Validation**: `ValidateLoginInputs()`
  - Validates Account ID: 3-50 characters
  - Validates Password: minimum 6 characters
  - Shows appropriate error messages

- **Input Clearing**: `ClearInputs()`
  - Clears all textboxes for next login attempt

#### Usage Example:
```csharp
// In a login button click handler
if (login.ValidateLoginInputs())
{
    // Process login
    MessageBox.Show("Login successful!");
}
else
{
    // Validation failed - user already notified
}
```

---

### 3. Kiosk Student Module (`ucKioskStudent.cs`)
Enhanced with comprehensive input validation for book checkout.

#### New Validation Methods:

**ValidateBorrowDays()**
- Ensures borrow days is a valid number
- Maximum 30 days allowed

**ValidateISBN(string isbn)**
- Validates ISBN format (10 or 13 characters)
- Ensures only numeric characters
- Prevents duplicate scans automatically

**ValidateCheckoutForm()**
- Ensures at least one book is scanned
- Validates borrow days before checkout
- Used before confirming checkout

#### Usage Example:
```csharp
// In checkout button handler
if (kioskStudent.ValidateCheckoutForm())
{
    // Process checkout
    MessageBox.Show("Checkout successful!");
}
```

---

## Kiosk Mode Feature

### How to Use Kiosk Mode:

1. **Access Kiosk Mode**: Click "Kiosk Mode" button on login page
2. **Scan Books**: Use barcode/QR code scanner to scan ISBN
3. **Automatic Book Lookup**: Book details appear automatically
4. **Adjust Borrow Days**: Use +/- buttons to set borrow period (max 30 days)
5. **Remove Books**: Click "Remove Book" to remove last scanned item
6. **Confirm Checkout**: Click "Checkout" to complete transaction

### Validation During Kiosk Mode:
- ISBN must be 10 or 13 digits
- Only numeric ISBN values accepted
- Duplicate ISBN detection prevents re-scanning
- Borrow days must be between 1-30
- At least one book required for checkout

---

## Implementation Examples

### Example 1: Login Form Validation
```csharp
private void btnLogin_Click(object sender, EventArgs e)
{
    if (ValidateLoginInputs())
    {
        // Proceed with authentication
        MessageBox.Show("Valid credentials!");
    }
    // Validation error already shown to user
}
```

### Example 2: Custom Textbox Validation
```csharp
private void ValidateCustomField()
{
    Guna.UI2.WinForms.Guna2TextBox myTextBox = new Guna.UI2.WinForms.Guna2TextBox();
    
    if (InputValidator.IsNotEmpty(myTextBox, "Full Name"))
    {
        if (InputValidator.IsAlphabeticOnly(myTextBox, "Full Name"))
        {
            if (InputValidator.HasMinimumLength(myTextBox, 3, "Full Name"))
            {
                // All validations passed
            }
        }
    }
}
```

### Example 3: Opening Kiosk Mode
```csharp
private void BtnKioskMode_Click(object sender, EventArgs e)
{
    try
    {
        KioskForm kioskForm = new KioskForm();
        this.Hide(); // Hide login form
        kioskForm.ShowDialog(); // Show kiosk as modal dialog
        this.Show(); // Show login form again
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

---

## Features Not Yet Implemented

As requested, the following features are NOT implemented yet:
- ❌ Login button functionality
- ❌ School logo display/upload
- ❌ School name configuration

These will be added in future updates.

---

## Error Handling

All validation methods include:
- Automatic focus on invalid field
- User-friendly error messages
- Icon indicators (Warning/Error)
- Clear, specific feedback on what went wrong

---

## Best Practices

1. **Always validate before database operations**
   ```csharp
   if (ValidateISBN(isbn))
   {
       // Query database
   }
   ```

2. **Chain validations logically**
   ```csharp
   if (InputValidator.IsNotEmpty(textbox, "Field"))
   {
       if (InputValidator.IsNumericOnly(textbox, "Field"))
       {
           // Further processing
       }
   }
   ```

3. **Clear sensitive data after use**
   ```csharp
   ClearInputs(); // Clears all textboxes
   ```

---

## Testing Recommendations

1. **Test Invalid Inputs**:
   - Empty fields
   - Out-of-range values
   - Special characters in numeric fields
   - Insufficient length
   - Duplicate entries

2. **Test Kiosk Flow**:
   - Open and close kiosk mode
   - Scan valid/invalid ISBNs
   - Adjust borrow days to limits
   - Remove scanned books
   - Complete checkout flow

3. **Test Edge Cases**:
   - Maximum borrow days (30)
   - ISBN with hyphens vs without
   - Multiple books in one session
   - Return to login after kiosk use

---

## Future Enhancements

- Email validation for account recovery
- Phone number validation
- Stronger password enforcement
- Session timeout after inactivity
- Input sanitization for SQL injection prevention
- Logging of all validation failures for security audit

---

Last Updated: 2024
Version: 1.0
