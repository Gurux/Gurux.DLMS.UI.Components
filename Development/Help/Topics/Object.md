# COSEM object

A COSEM object represents one piece of device data or one device function. Its class determines which attributes it has. The logical name identifies the individual object in the device.

## When to use it

Use the general object view when a device exposes a class without a dedicated editor. Read the values before editing them and check the access rights for your connection. The object version determines which attributes are available.

## Attributes

### 1. Logical name

The six-part OBIS identifier of this object, such as 0.0.1.0.0.255. It identifies the instance within the device; it is separate from the class identifier.

### Other attributes

Each class defines its own additional attributes. The general view shows the attribute names and values provided by the device library. The class help explains them when a description is available.
