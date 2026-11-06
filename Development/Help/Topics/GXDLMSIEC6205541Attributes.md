# STS payment token settings

<!-- cosem-attributes: 12 -->

This object exposes the identity and key management settings of a meter that accepts STS payment tokens. It helps a payment system select tokens compatible with that meter.

## When to use it

Use it when commissioning a token payment meter or checking its supply group and key revision before issuing tokens.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Meter PAN

Identifies the meter's primary account number using the issuer identifier, decoder reference and check digit. It is set during manufacture.

### 3. Commodity

Identifies what the meter supplies, such as electricity, water, gas or time. Together with the account currency it determines token units.

### 4. Token carrier types

Lists the supported ways in which tokens can be carried or presented to the meter.

### 5. Encryption algorithm

Identifies the algorithm used to protect payment token information.

### 6. Supply group code

Identifies the meter's supply group for token key management. Key change tokens can update it.

### 7. Tariff index

Identifies the tariff selection used in token key management. It is updated through key change tokens.

### 8. Key revision number

Identifies the revision of the token decoding key. It is updated through key change tokens.

### 9. Key type

Identifies the type of token decoding key currently used by the meter.

### 10. Key expiry number

Identifies the expiry setting associated with the token decoding key. It is updated through key change tokens.

### 11. Supported key change tokens

Shows how many tokens the meter supports in a key change sequence.

### 12. STS certificate

Holds the meter's STS conformance certificate number. It can change when firmware is upgraded.
