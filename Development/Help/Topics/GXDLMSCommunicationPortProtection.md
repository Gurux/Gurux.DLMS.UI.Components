# Communication port protection

<!-- cosem-attributes: 10 -->

This object protects a communication port by blocking access after repeated failed attempts. It also records failures and shows whether the port is currently locked.

## When to use it

Use it to limit repeated unsuccessful access attempts on a meter's remote communication port.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Protection mode

Selects permanent locking, locking after failed attempts or unrestricted access through the port.

### 3. Allowed failed attempts

Sets how many failed communication attempts are allowed before temporary locking starts.

### 4. Initial lockout time

Sets the initial temporary lock duration in seconds once the failure limit is reached.

### 5. Steepness factor

Controls how quickly lock durations grow as further failed attempts occur.

### 6. Maximum lockout time

Limits the duration of a temporary lock, in seconds.

### 7. Port

Identifies the object representing the protected port. The reference can be empty when a separate port identification is not needed.

### 8. Protection status

Shows whether the port is unlocked, temporarily locked or permanently locked.

### 9. Failed attempts

Counts failed communication attempts since the last reset, regardless of the protection mode.

### 10. Cumulative failed attempts

Keeps the total failure count independently of the resettable failed attempts counter.
