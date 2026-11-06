# M-Bus client

<!-- cosem-attributes: 12,14 -->

Represents an M-Bus device read by a DLMS meter or gateway. It links the communication port, selected data records and identity of the attached device.

## When to use it

For example, configure a gateway to collect water meter readings every hour and map them to its DLMS objects.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. MBus Port Reference

Identifies the M-Bus port object used to communicate with the attached device.

### 3. Capture Definition

Identifies the M-Bus data records to capture using their data-information and value-information blocks.

### 4. Capture Period

Sets the interval between automatic data collections in seconds. A zero value disables periodic collection.

### 5. Primary Address

Contains the short M-Bus address used to select the attached device.

### 6. Identification Number

Contains the identification number reported by the attached device.

### 7. Manufacturer ID

Contains the manufacturer code reported in the M-Bus device identity.

### 8. Version

Contains the M-Bus product or device revision reported by the attached device.

### 9. Device Type

Identifies the type of device or measured medium, such as water, heat or gas.

### 10. Access Number

Contains the access number from the last M-Bus response, which helps identify successive telegrams.

### 11. Status

Contains status flags reported by the attached device, such as temporary or permanent errors.

### 12. Alarm

Contains alarm information reported by the attached device.

### 13. Configuration

Shows the configuration field received from the M-Bus device, including its encryption mode and number of encrypted bytes. Updated on each device readout.

### 14. Encryption Key Status

Reports the state of the encryption key used with the attached device.
