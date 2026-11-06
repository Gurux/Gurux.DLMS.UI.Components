# M-Bus diagnostics

<!-- cosem-attributes: 9 -->

Shows the health and traffic statistics of an M-Bus communication link.

## When to use it

For example, compare received signal strength and failed frames when investigating missed wireless meter readings.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. Received signal strength

Shows the strength of the last received wireless M-Bus signal in dBm. This applies to bidirectional wireless communication.

### 3. Channel Id

Identifies the channel on which communication is taking place.

### 4. Link status

Reports the current condition of the communication link.

### 5. Broadcast frames

Records broadcast reception counts separately for each client identifier, together with the time of the last received frame.

### 6. Transmissions

Counts frames transmitted by the interface.

### 7. Received frames

Counts received frames that passed their checksum check.

### 8. Failed received frames

Counts received frames that failed their checksum check.

### 9. Capture time

Identifies the most recently changed diagnostic attribute and records when it changed.
