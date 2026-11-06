# Automatic connection

<!-- cosem-attributes: 6,6,6 -->

This object controls outgoing connections or messages initiated by a meter. It defines connection timing, retry behavior and destinations.

## When to use it

Use it to arrange a meter's scheduled connection to a collection system and retry after a failed attempt.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

<!-- versions: 0 -->
### 2. Mode (version 0)

Selects whether automatic dialling is disabled, always allowed, limited to the calling window, or allowed outside the window for alarms. Values 200-255 are manufacturer specific.
<!-- /versions -->

<!-- versions: 1 -->
### 2. Mode (version 1)

Selects automatic dialling timing or delivery by SMS over the mobile or telephone network, or by email. Values 200-255 are manufacturer specific.
<!-- /versions -->

<!-- versions: 2 -->
### 2. Mode (version 2)

Selects dialling and message delivery behavior, or packet network connection modes 101-104. Packet modes keep the device connected permanently or during the calling window, or connect when the connect method is invoked. Values 200-255 are manufacturer specific.
<!-- /versions -->

### 3. Repetitions

Sets the maximum number of retries after unsuccessful connection attempts.

### 4. Repetition delay

Sets the wait in seconds before retrying a failed connection. Zero leaves the delay unspecified.

### 5. Calling window

Lists the periods during which outgoing connection activity is allowed, subject to the selected mode.

<!-- versions: 0 -->
### 6. Phone list (version 0)

Lists telephone numbers the modem calls for outgoing communication.
<!-- /versions -->

<!-- versions: 1-2 -->
### 6. Destinations (versions 1-2)

Lists remote destinations, such as telephone numbers or email addresses, used for outgoing communication.
<!-- /versions -->

