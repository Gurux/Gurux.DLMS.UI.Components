# LLC type 3 setup

<!-- cosem-attributes: 6 -->

Configures a connectionless logical link service that acknowledges individual exchanges.

## When to use it

For example, increase the response wait when a network adds a predictable transmission delay.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. MaximumOctetsACnPdu

Sets the largest payload, in bytes, in an acknowledged connectionless command.

### 3. MaximumTransmissions

Limits the number of attempts to send a command and obtain its response.

### 4. AcknowledgementTime

Sets how many seconds to wait for the response to an acknowledged command.

### 5. ReceiveLifetime

Sets how long receive state is retained, allowing repeated copies of the same exchange to be recognised.

### 6. TransmitLifetime

Sets how long transmit state is retained before its sequence information can be reused.
