# Data protection

<!-- cosem-attributes: 6 -->

Data protection applies security to a selected group of object attributes. It allows those values to be exchanged as a protected payload.

## When to use it

Use it when a group of readings or settings must retain protection while passing through an intermediate system.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Buffer

Contains the protected payload. Reading captures and protects the configured attributes; writing verifies the payload before applying its values.

### 3. Object list

Lists the objects, attributes and optional selections included in the protected exchange.

### 4. Get parameters

Defines the security and key parameters used to protect data returned by a read.

### 5. Set parameters

Defines the security and key parameters used to verify and process written data.

### 6. Required protection

Specifies the protection required for protected reads, writes and method requests.
