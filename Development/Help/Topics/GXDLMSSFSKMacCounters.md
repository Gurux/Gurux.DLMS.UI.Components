# S-FSK MAC counters

<!-- cosem-attributes: 8 -->

Records synchronisation history, repeated traffic and frame integrity results on an S-FSK network.

## When to use it

For example, inspect synchronisation losses and CRC failures when a meter repeatedly disappears from the network.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. SynchronizationRegister

Records how often the device has synchronised with each initiator.

### 3. Desynchronization listing

Counts losses of synchronisation according to their cause, helping distinguish timeouts from other failures.

### 4. BroadcastFramesCounter

Counts received broadcast frames separately for each source MAC address.

### 5. RepetitionsCounter

Counts repetition phases in which the device forwards a received frame as a repeater. Several copies sent during one repetition phase count only once.

### 6. TransmissionsCounter

Counts transmission phases initiated by the device, including repetitions of its own frame. Forwarding a received frame is counted separately as a repetition phase.

### 7. CrcOkFramesCounter

Counts received frames that passed their CRC integrity check.

### 8. CrcNOkFramesCounter

Counts received frames that failed their CRC integrity check.
