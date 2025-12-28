# Season Wizard Bug Fix Summary

## Problem Identified

The Season Creation Wizard was stuck on step 2 because the `CurrentStep` hidden field was using ASP.NET model binding (`asp-for="CurrentStep"`), which caused it to retain the value from the PREVIOUS form submission rather than the CURRENT step being displayed.

## Root Cause

### The Bug Flow
1. User on step 1, clicks Next
2. Controller validates step 1, increments CurrentStep to 2, returns view
3. View displays step 2 content
4. **BUG**: Hidden field `<input asp-for="CurrentStep" />` still contains value 1 (from model binding)
5. User fills step 2, clicks Next
6. Form submits with CurrentStep = 1 (wrong!)
7. Controller validates step 1 again (not step 2)
8. Controller "advances" from 1 to 2
9. User sees step 2 again, appearing stuck

### Evidence from Logs
```
info: Wizard POST - Step: 1, Action: next, Year: 2026
info: Validating wizard step 1 for year 2026
info: Step 1 validation result: True
info: Moving to next step: 2
```

Even though the user was on step 2 and clicked Next, the logs show "Step: 1" being validated.

## The Fix

### Changed From (Broken)
```html
<input type="hidden" asp-for="CurrentStep" />
```

### Changed To (Fixed)
```html
<input type="hidden" name="CurrentStep" value="@Model.CurrentStep" />
```

### Why This Works
- `asp-for` uses model binding, which gets the value from the POST data
- `value="@Model.CurrentStep"` uses the actual current step from the model
- The model's CurrentStep is updated by the controller before rendering
- The hidden field now correctly reflects the displayed step

## Files Modified

1. **src/NflPlayoffPool.Web/Areas/Admin/Views/Season/Wizard.cshtml**
   - Changed CurrentStep hidden field from `asp-for` to explicit `value` attribute

2. **src/NflPlayoffPool.Web/Areas/Admin/Controllers/SeasonController.cs**
   - Added comprehensive logging for debugging (can be kept or removed)

## Testing Instructions

1. Navigate to http://localhost:8080
2. Log in as admin (admin@nflplayoffpool.local / Password123!@#)
3. Go to Admin → Seasons → Create Season Wizard
4. Enter year 2026 on step 1, click Next
5. Verify you reach step 2 (Scoring Rules)
6. Enter or keep default scoring values, click Next
7. **Expected**: You should now reach step 3 (Teams Setup)
8. Continue through step 4 and create the season

## Verification

Check the application logs after testing:
```bash
docker logs nflpool-webapp --tail 50
```

You should see logs showing:
- Step 2 being validated when on step 2
- Step 3 being validated when on step 3
- Successful progression through all steps

## Additional Improvements Made

1. **Enhanced Logging**: Added detailed logging to track:
   - Current step and action
   - Field values being submitted
   - Validation results
   - ModelState errors

2. **Better Error Display**: Added validation summary that shows errors clearly

3. **Debugging Support**: Logs now show exactly what's happening at each step

## Prevention

To prevent similar issues in the future:
- Be careful with `asp-for` binding on hidden fields that change during processing
- Use explicit `value` attributes when the field should reflect computed/updated values
- Add logging to track form submission values vs. displayed values
- Test wizard flows thoroughly with logging enabled

## Rollback Plan

If this fix causes issues, revert the change in Wizard.cshtml:
```html
<!-- Revert to -->
<input type="hidden" asp-for="CurrentStep" />
```

Then investigate alternative solutions in the controller logic.

## Status

✅ **Fix Applied**: Hidden field now uses explicit value
✅ **Application Rebuilt**: Docker container updated
✅ **Ready for Testing**: Application running at http://localhost:8080

**Next Step**: Test the wizard to confirm the fix works correctly.