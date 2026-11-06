# LLC type 2 setup

<!-- cosem-attributes: 9 -->

Configures a connection based logical link service with acknowledgements, flow control and retries.

## When to use it

For example, adjust the acknowledgement timer on a slow link so normal delays do not cause unnecessary retransmissions.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. TransmitWindowSizeK

Limits the number of sent information packets that may be awaiting acknowledgement.

### 3. TransmitWindowSizeRW

Sets the receive window: the number of information packets the peer may send before it must wait for acknowledgement. Despite its displayed name, this is a receive setting.

### 4. MaximumOctetsPdu

Sets the largest information packet payload in bytes.

### 5. MaximumNumberTransmissions

Limits repeated transmissions following a response or recovery timeout.

### 6. AcknowledgementTimer

Sets how many seconds to wait for acknowledgement of sent information or an expected command response.

### 7. BitTimer

Sets how many seconds to wait for a final response after sending a command with its poll bit set.

### 8. RejectTimer

Sets how many seconds to wait for recovery after requesting retransmission of rejected information.

### 9. BusyStateTimer

Sets how many seconds to wait for the peer to report that it is no longer busy.
