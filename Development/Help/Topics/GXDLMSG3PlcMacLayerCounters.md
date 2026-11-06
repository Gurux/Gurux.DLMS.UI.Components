# G3-PLC MAC layer counters

<!-- cosem-attributes: 10,10 -->

Records traffic and communication problems at the G3 power line link layer. Comparing readings over time helps reveal congestion or poor links.

## When to use it

For example, compare missing acknowledgements and checksum errors before and after repairing a noisy connection.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. TxDataPacketCount

Counts data packets successfully sent.

### 3. RxDataPacketCount

Counts data packets successfully received.

### 4. TxCmdPacketCount

Counts link management command packets successfully sent.

### 5. RxCmdPacketCount

Counts link management command packets successfully received.

### 6. CSMAFailCount

Counts attempts abandoned after reaching the channel access backoff limit.

### 7. CSMANoAckCount

Counts unicast data transmissions for which the expected acknowledgement did not arrive.

### 8. BadCrcCount

Counts received frames that failed the CRC integrity check.

### 9. TxDataBroadcastCount

Counts broadcast data frames sent.

### 10. RxDataBroadcastCount

Counts broadcast data frames successfully received.
