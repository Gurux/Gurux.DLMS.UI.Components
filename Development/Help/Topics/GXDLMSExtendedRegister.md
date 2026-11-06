# Extended register

<!-- cosem-attributes: 5 -->

An extended register adds status and a capture timestamp to a scaled measurement. This helps a reader interpret both the value and the circumstances of its capture.

## When to use it

Use it for a captured maximum or a billing reading that needs an associated status and time.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Value

Contains the measured or captured value.

### 3. Scaler and unit

Defines the power-of-ten multiplier and physical unit applied to the value.

### 4. Status

Contains status information associated with the value.

### 5. Capture time

Records when the value was captured.
