# Demand register

<!-- cosem-attributes: 9 -->

A demand register calculates an average measurement over a defined interval. It keeps the current calculation and the result of the previous completed interval.

## When to use it

Use it to inspect average electrical demand over successive 15-minute intervals.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Current average value

Shows the average being calculated for the current interval.

### 3. Last average value

Shows the average from the last completed interval.

### 4. Scaler and unit

Defines the power-of-ten multiplier and physical unit for the values.

### 5. Status

Provides status information associated with the measurement.

### 6. Capture time

Records the capture time associated with the last completed average.

### 7. Start time current

Records when the current averaging interval began.

### 8. Period

Defines the averaging period in seconds.

### 9. Number of periods

Defines how many periods are used in the demand calculation, including sliding-demand calculations.
