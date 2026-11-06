# PRIME MAC setup

<!-- cosem-attributes: 8 -->

Configures how a PRIME node discovers switches, advertises beacons and retries link control messages.

## When to use it

For example, adjust discovery and retry timing when commissioning service nodes on a busy power line.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. MacMinSwitchSearchTime

Sets how many seconds a disconnected service node scans for beacons before requesting switch promotion.

### 3. MacMaxPromotionPdu

Limits the number of promotion request packets that a service node may send within the configured period.

### 4. MacPromotionPduTxPeriod

Sets the period, in seconds, over which the promotion request packet limit applies.

### 5. MacBeaconsPerFrame

Limits the number of beacon slots that a base node may allocate in each frame.

### 6. MacScpMaxTxAttempts

Limits channel access attempts when earlier attempts found the channel busy.

### 7. MacCtlReTxTimer

Sets how many seconds to wait for acknowledgement of a link control packet before retrying.

### 8. MacMaxCtlReTx

Limits retransmissions of a link control packet that has not been acknowledged.
