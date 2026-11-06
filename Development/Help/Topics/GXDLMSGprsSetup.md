# GPRS setup

<!-- cosem-attributes: 4 -->

This object holds the settings a cellular modem needs to obtain a GPRS packet data connection. It includes the mobile network access point and requested service characteristics.

## When to use it

Use it when provisioning a meter's SIM and mobile data service for communication with a collection system.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. APN

Sets the access point name used to reach the mobile packet data network.

### 3. PIN code

Holds the SIM personal identification number needed by the modem.

### 4. Default and requested quality of service

Holds separate default and requested service parameters. Each set describes precedence, delay, reliability and throughput.
