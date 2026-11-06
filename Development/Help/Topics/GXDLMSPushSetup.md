# Push setup

<!-- cosem-attributes: 7,10,13 -->

Push setup defines which object values a device sends automatically, where it sends them and how delivery is retried. It supports event-driven reporting without a separate read request for every value.

## When to use it

Use it to send a daily reading or an alarm report to a collection server.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Object list

Lists the objects, attributes and optional element selections included in the push message.

### 3. Send destination and method

Defines the transport service, destination address and message representation.

### 4. Communication window

Lists the time windows during which push communication is allowed.

### 5. Randomisation start interval

Sets the maximum initial random delay in seconds, helping spread traffic from many devices.

### 6. Number of retries

Sets how many retries may follow an unsuccessful attempt.

<!-- versions: 0-1 -->
### 7. Repetition delay (versions 0-1)

Sets a single delay, in seconds, between push delivery attempts.
<!-- /versions -->

<!-- versions: 2 -->
### 7. Repetition delay (version 2)

Defines the minimum delay, exponential growth factor and maximum delay used to schedule successive push delivery attempts.
<!-- /versions -->

### 8. Port reference

References the communication port used for the push.

### 9. Push client sap

Specifies the client service access point used for the push destination.

### 10. Push protection parameters

Defines the security protection applied to the pushed data.

### 11. Push operation method

Selects the push delivery and confirmation behavior.

### 12. Confirmation parameters

Defines the timing parameters for confirming a push.

### 13. Last confirmation date time

Records the date and time of the most recently confirmed push.

## Versions

Version 0 provides attributes 1-7. Version 1 adds attributes 8-10. Version 2 adds attributes 11-13 and changes attribute 7 to a structured retry-delay configuration.
