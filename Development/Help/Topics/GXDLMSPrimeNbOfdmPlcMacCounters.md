# PRIME MAC counters

<!-- cosem-attributes: 7 -->

Records successful traffic and channel access problems in the PRIME link layer.

## When to use it

For example, inspect busy channel and access failure counts to investigate congestion.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. TxDataPktCount

Counts data packets successfully transmitted.

### 3. RxDataPktCount

Counts data packets successfully received.

### 4. TxCtrlPktCount

Counts link control packets successfully transmitted.

### 5. RxCtrlPktCount

Counts link control packets successfully received.

### 6. CsmaFailCount

Counts transmission requests that failed to gain channel access after the permitted attempts.

### 7. CsmaChBusyCount

Counts channel access attempts that found the channel busy.
