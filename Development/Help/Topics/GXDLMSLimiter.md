# Limiter

<!-- cosem-attributes: 11 -->

A limiter watches a measurement against a threshold and can trigger actions after a sustained crossing. It supports normal and emergency operating limits.

## When to use it

Use it to limit power when demand remains above a permitted level for a configured time.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Monitored value

Identifies the source object, attribute and optional element being monitored.

### 3. Active threshold

Shows the threshold currently in effect.

### 4. Normal threshold

Defines the threshold used during normal operation.

### 5. Emergency threshold

Defines the threshold used while an emergency profile is active.

### 6. Threshold duration min over

Sets how long the measurement must remain above the limit before the over-limit action occurs.

### 7. Threshold duration min under

Sets how long the measurement must remain below the limit before the under-limit action occurs.

### 8. Emergency profile

Defines an emergency profile identifier, start time and duration.

### 9. Emergency profile group

Lists the emergency profile identifiers applicable to this limiter.

### 10. Emergency profile active

Shows whether emergency limiting is currently active.

### 11. Actions

Defines the scripts to execute for sustained over-limit and under-limit conditions.
