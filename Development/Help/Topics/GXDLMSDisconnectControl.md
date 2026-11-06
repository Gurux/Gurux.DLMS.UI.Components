# Disconnect control

<!-- cosem-attributes: 4,4 -->

Disconnect control represents a supply switch and the rules governing disconnection and reconnection. It distinguishes the physical output from the control state.

## When to use it

Use it to check whether supply is disconnected and whether reconnection requires a local action.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Output state

Shows whether the physical output is connected or disconnected.

### 3. Control state

Shows the control state, such as disconnected, connected or ready for reconnection.

<!-- versions: 0 -->
### 4. Control mode (version 0)

Selects modes 0-6, defining the allowed disconnection and reconnection transitions and the roles of remote, manual and local control.
<!-- /versions -->

<!-- versions: 1 -->
### 4. Control mode (version 1)

Selects modes 0-7, defining the allowed disconnection and reconnection transitions. Mode 7 permits local or remote reconnection from ready for reconnection to connected, as well as manual reconnection.
<!-- /versions -->

## Versions

Versions 0 and 1 have the same attribute indexes. Version 1 extends the available control modes; interpret the mode value according to the object version.
