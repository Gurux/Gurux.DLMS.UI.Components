# HDLC setup

<!-- cosem-attributes: 9,9 -->

This object configures the meter's HDLC communication port. It defines the line speed, addressing, frame limits and timeouts used for a serial connection.

## When to use it

Use it when matching a reading application's serial settings to a meter or adjusting frame sizes for a constrained link.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Communication speed

Sets the port's communication rate. Another protocol's entry procedure may override this rate.

### 3. Window size transmit

Sets how many frames the meter may send before receiving an acknowledgement. Connection setup can negotiate another value.

### 4. Window size receive

Sets how many frames the meter may receive before sending an acknowledgement. Connection setup can negotiate another value.

<!-- versions: 0 -->
### 5. Maximum information length transmit (version 0)

Limits the information field in transmitted frames to 32-128 bytes. A smaller size can be agreed during connection setup.
<!-- /versions -->

<!-- versions: 1 -->
### 5. Maximum information length transmit (version 1)

Limits the information field in transmitted frames to 32-2030 bytes. A smaller size can be agreed during connection setup.
<!-- /versions -->

<!-- versions: 0 -->
### 6. Maximum information length receive (version 0)

Limits the information field in received frames to 32-128 bytes. A smaller size can be agreed during connection setup.
<!-- /versions -->

<!-- versions: 1 -->
### 6. Maximum information length receive (version 1)

Limits the information field in received frames to 32-2030 bytes. A smaller size can be agreed during connection setup.
<!-- /versions -->

<!-- versions: 0 -->
### 7. Intercharacter timeout (version 0)

Sets the gap in milliseconds after which received bytes are treated as a complete frame. The configured range is 20-1000 milliseconds.
<!-- /versions -->

<!-- versions: 1 -->
### 7. Intercharacter timeout (version 1)

Sets the gap in milliseconds after which received bytes are treated as a complete frame. The configured range is 20-6000 milliseconds.
<!-- /versions -->

### 8. Inactivity timeout

Sets how many seconds without a received frame cause disconnection. Zero disables this timeout.

### 9. Device address

Holds the physical HDLC address used to reach the device.
