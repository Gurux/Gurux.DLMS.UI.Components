# CoAP diagnostics

<!-- cosem-attributes: 5 -->

This object reports activity and problems in the meter's CoAP communication layers. Its counters help distinguish message delivery problems from request handling or block transfer failures.

## When to use it

Use it when a CoAP connection is slow or unreliable and you need to check retries, errors or incomplete transfers.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Messages counter

Groups counts of sent, received and repeated messages, acknowledgements, resets and dropped messages. It also reports responses sent separately from acknowledgements and abandoned transmissions.

### 3. Request response counter

Groups counts of requests, successful responses and client or server errors in both directions. These counts describe application exchanges rather than retransmissions.

### 4. Block transfer counter

Counts block transfers that started, completed or timed out. Use it to find failures when larger messages are split into blocks.

### 5. Capture time

Records an attribute identifier and the time of the most recent recorded counter change or reset.
