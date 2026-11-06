# Optical port setup

<!-- cosem-attributes: 9,9 -->

This object configures the meter's optical communication port, including its opening procedure, negotiated speed and local access passwords.

## When to use it

Use it when setting up an optical reading probe and checking the initial baud rate and meter address.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Default mode

Selects the protocol behavior used when communication starts on the port.

### 3. Default baud rate

Sets the communication rate used for the initial opening exchange.

### 4. Proposed baud rate

Sets the communication rate proposed by the meter for the session after the opening exchange.

### 5. Response time

Sets the minimum delay between receiving a request and beginning the response.

### 6. Device address

Holds the address used to select the meter during local communication.

### 7. Password 1

Holds the password for the local protocol's P1 password operation.

### 8. Password 2

Holds the password for the local protocol's P2 password operation.

### 9. Password 5

Holds the W5 password reserved for national applications.
