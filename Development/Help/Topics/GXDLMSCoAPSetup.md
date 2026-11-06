# CoAP setup

<!-- cosem-attributes: 13 -->

This object configures the CoAP layer used to carry meter communication over UDP. It controls acknowledgements, retries, message concurrency and wrapper details.

## When to use it

Use it when configuring a CoAP endpoint or adjusting retry timing for a network with long delays.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. UDP reference

Identifies the TCP-UDP setup object providing the underlying UDP endpoint and port.

### 3. Acknowledgement timeout

Sets the minimum initial wait for acknowledgement of a confirmable message, in milliseconds.

### 4. Acknowledgement random factor

Sets the random variation applied to the initial acknowledgement timeout. The stored factor is expressed in hundredths.

### 5. Maximum retransmissions

Limits how many times a confirmable message can be sent again without acknowledgement.

### 6. NStart

Limits the number of requests that may remain outstanding at the same time.

### 7. Delayed acknowledgement timeout

Sets how long, in milliseconds, to wait for an application response before sending a separate acknowledgement.

### 8. Exponential backoff

Sets the factor that increases the delay between successive retransmissions. The stored factor is expressed in hundredths.

### 9. Probing rate

Limits the average number of bytes per second sent to an endpoint that does not respond.

### 10. CoAP URI path

Sets the resource path used by the meter communication wrapper.

### 11. Transport mode

Selects support for reliable operation, unreliable operation or both.

### 12. Wrapper version

Identifies the version of the CoAP wrapper used for meter messages.

### 13. Token length

Sets the number of bytes used for CoAP client tokens that match responses to requests.
