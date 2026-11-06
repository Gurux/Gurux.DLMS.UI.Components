# Cellular modem diagnostics

<!-- cosem-attributes: 8,8,8 -->

This object reports mobile network registration, active services and radio cell information. It helps explain whether a meter's modem can communicate through the cellular network.

## When to use it

Use it to investigate a meter that cannot register with its operator or has poor radio reception.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Operator

Shows the name of the mobile network operator.

### 3. Status

Shows modem registration state, such as searching, registered at home, roaming or denied.

### 4. Circuit switch status

Shows whether the circuit switched service is inactive, receiving a call or active.

<!-- versions: 0 -->
### 5. Packet switch status (version 0)

Reports inactive service, GPRS, EDGE, UMTS or HSDPA.
<!-- /versions -->

<!-- versions: 1-2 -->
### 5. Packet switch status (versions 1-2)

Reports inactive service, GPRS, EDGE, UMTS, HSDPA, LTE or CDMA.
<!-- /versions -->

<!-- versions: 0 -->
### 6. Cell information (version 0)

Describes the serving cell using a 16-bit cell identifier, location area code, signal quality and bit error rate.
<!-- /versions -->

<!-- versions: 1-2 -->
### 6. Cell information (versions 1-2)

Describes the serving cell using a 32-bit cell identifier, location or tracking area code, signal quality, bit error rate, mobile country code, mobile network code and radio channel number.
<!-- /versions -->

<!-- versions: 0 -->
### 7. Adjacent cells (version 0)

Lists nearby cells using 16-bit cell identifiers and their signal quality.
<!-- /versions -->

<!-- versions: 1-2 -->
### 7. Adjacent cells (versions 1-2)

Lists nearby cells using 32-bit cell identifiers and their signal quality.
<!-- /versions -->

### 8. Capture time

Shows when the diagnostic information was last captured.
