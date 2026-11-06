# M-Bus port setup

<!-- cosem-attributes: 11 -->

Configures an M-Bus port through which a device provides its DLMS server. It includes addressing, packet size and listening periods.

## When to use it

For example, configure a battery powered wireless meter to listen during scheduled communication windows.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. Profile selection

Selects the wired or wireless M-Bus communication profile used by the port.

### 3. Port communication status

Reports whether the port is available for communication and its current communication state.

### 4. Data header type

Selects the format of the M-Bus header carried before the application data.

### 5. Primary address

Sets the short M-Bus address used to address the device.

### 6. Identification number

Contains the device identification number used in its M-Bus identity.

### 7. Manufacturer Id

Contains the manufacturer code in the M-Bus identity.

### 8. MBus version

Contains the device revision used in the M-Bus identity.

### 9. Device type

Identifies the device category or measured medium in the M-Bus identity.

### 10. Max PDU size

Sets the largest protocol data unit that the port can handle.

### 11. Listening window

Lists the time periods during which the port listens for incoming communication.
