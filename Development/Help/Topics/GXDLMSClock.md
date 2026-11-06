# Clock

<!-- cosem-attributes: 9 -->

A clock exposes the device date and time together with time-zone, daylight-saving and clock-quality settings. Other objects use this time for schedules and recorded timestamps.

## When to use it

Use it to check a meter clock, correct its time or configure seasonal clock changes.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Time

Shows the current device date and time, including the associated deviation and status fields.

### 3. Time zone

Sets the minutes added to local normal time to obtain UTC, excluding daylight saving. For a normal time zone of UTC+2, the value is -120.

### 4. Status

Reports clock-quality flags, such as invalid or doubtful time and active daylight saving.

### 5. Begin

Defines when the daylight-saving period begins.

### 6. End

Defines when the daylight-saving period ends.

### 7. Deviation

Defines the daylight-saving adjustment in minutes.

### 8. Enabled

Enables or disables the daylight-saving calculation.

### 9. Clock base

Identifies the source on which the device clock is based.
