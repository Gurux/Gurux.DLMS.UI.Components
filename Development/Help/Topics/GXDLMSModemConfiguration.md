# Modem configuration

<!-- cosem-attributes: 4 -->

This object configures communication between a meter and its modem. It supplies the serial rate, startup commands and modem-specific response strings.

## When to use it

Use it when fitting a modem that needs a particular initialization sequence or uses different response text.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Communication speed

Sets the rate between the meter and modem. This may differ from the speed of the external network connection.

### 3. Initialization strings

Lists startup commands and their expected responses, in execution order. Each entry includes the wait in milliseconds after its matching response before the next command is sent.

### 4. Modem profile

Maps familiar modem responses such as connect, busy or no carrier to the text used by the installed modem.
