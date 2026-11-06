# Token gateway

<!-- cosem-attributes: 6 -->

A token gateway receives and processes tokens for a payment application. It reports the received token, its origin and the processing result.

## When to use it

Use it to check whether a prepaid top-up token was accepted or rejected.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Token

Contains the token submitted to the gateway.

### 3. Time

Records the date and time associated with receiving the token.

### 4. Description

Contains descriptive information associated with the token.

### 5. Delivery method

Identifies how the token was delivered, such as remote or local entry.

### 6. Status

Reports the processing result and associated result data.
