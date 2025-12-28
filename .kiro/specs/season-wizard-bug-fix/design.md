# Design Document

## Overview

The Season Creation Wizard bug is caused by incorrect handling of the CurrentStep hidden field. The field contains the step number from the previous form submission rather than the current step being displayed, causing validation to run against the wrong step.

## Root Cause Analysis

### The Problem Flow
1. User loads wizard (CurrentStep = 1)
2. User fills step 1, clicks Next
3. Controller validates step 1, increments to step 2, returns view
4. View displays step 2 content, but hidden field still contains CurrentStep = 1
5. User fills step 2, clicks Next  
6. Form submits with CurrentStep = 1 (from hidden field)
7. Controller validates step 1 again instead of step 2
8. Controller "advances" from step 1 to step 2
9. User sees step 2 again, appearing stuck

### Technical Details
- The `<input type="hidden" asp-for="CurrentStep" />` field is bound during model binding
- Model binding occurs BEFORE the controller logic runs
- The controller updates CurrentStep after validation, but the hidden field retains the old value
- This creates a mismatch between displayed step and submitted step

## Solution Design

### Approach 1: Fix Hidden Field Value (Recommended)
Update the hidden field to always reflect the currently displayed step, not the submitted step.

**Implementation:**
```html
<!-- Instead of asp-for binding, use explicit value -->
<input type="hidden" name="CurrentStep" value="@Model.CurrentStep" />
```

### Approach 2: Controller Logic Fix (Alternative)
Modify controller to handle the step mismatch by using the displayed step for validation.

**Implementation:**
- Add logic to determine actual current step from view state
- Validate against displayed step rather than submitted step

## Architecture

### Components Affected
- **Wizard.cshtml**: Hidden field binding
- **SeasonController.Wizard**: POST method validation logic
- **SeasonWizardViewModel**: CurrentStep property handling

### Data Flow
```
User Input → Form Submission → Model Binding → Validation → Step Increment → View Render
```

**Current (Broken) Flow:**
- Model Binding sets CurrentStep = previous step
- Validation runs against previous step
- Step increment creates mismatch

**Fixed Flow:**
- Hidden field always contains current displayed step
- Validation runs against correct step
- Step progression works correctly

## Implementation Plan

### Phase 1: Immediate Fix
1. Update hidden field in Wizard.cshtml to use explicit value
2. Test wizard progression through all steps
3. Verify data persistence across steps

### Phase 2: Validation Enhancement  
1. Add step validation logging for verification
2. Ensure error messages display correctly
3. Test edge cases and error scenarios

### Phase 3: Robustness Improvements
1. Add client-side validation consistency checks
2. Implement step state validation
3. Add comprehensive error handling

## Testing Strategy

### Unit Tests
- Test CurrentStep handling in controller
- Test validation logic for each step
- Test model binding scenarios

### Integration Tests  
- Test complete wizard flow
- Test step navigation (next/previous)
- Test data persistence across steps
- Test error handling and validation display

### Manual Testing
- Test wizard with valid data through all steps
- Test validation errors on each step
- Test browser back/forward navigation
- Test form submission edge cases

## Error Handling

### Validation Errors
- Display clear error messages for each field
- Highlight invalid fields visually
- Preserve user input when validation fails

### Step State Errors
- Handle invalid step transitions gracefully
- Prevent direct navigation to invalid steps
- Maintain data integrity across step changes

## Performance Considerations

### Minimal Impact
- Hidden field change has no performance impact
- Validation logic remains unchanged
- No additional database queries required

### Optimization Opportunities
- Consider client-side validation for immediate feedback
- Implement progressive enhancement for better UX
- Add loading states for form submissions

## Security Considerations

### Form Security
- Maintain anti-forgery token protection
- Validate all user input server-side
- Prevent step manipulation attacks

### Data Protection
- Ensure sensitive data is not exposed in hidden fields
- Validate step transitions server-side
- Maintain proper authorization checks

## Rollback Plan

If the fix causes issues:
1. Revert hidden field change
2. Implement alternative controller-based fix
3. Add additional validation logging
4. Test thoroughly before re-deployment

The fix is minimal and low-risk, making rollback straightforward if needed.