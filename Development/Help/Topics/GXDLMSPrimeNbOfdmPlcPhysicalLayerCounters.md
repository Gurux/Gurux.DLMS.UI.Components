# PRIME physical layer counters

<!-- cosem-attributes: 5 -->

Records corrupted bursts and queue overflows in the PRIME physical layer.

## When to use it

For example, distinguish electrical noise from queue overload by comparing CRC errors with dropped data counts.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. CrcIncorrectCount

Counts received bursts whose CRC integrity check failed.

### 3. CrcFailedCount

Counts bursts whose CRC passed but whose physical header contained an invalid protocol value, revealing an error missed by the CRC.

### 4. TxDropCount

Counts occasions when the transmit queue was full and new or queued data had to be discarded.

### 5. RxDropCount

Counts occasions when the receive queue was full and newly received or queued data had to be discarded.
