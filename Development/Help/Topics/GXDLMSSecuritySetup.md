# Security setup

<!-- cosem-attributes: 5,6 -->

This object manages application communication protection for one or more associations. It identifies the required security policy, cryptographic suite and communicating systems.

## When to use it

Use it when provisioning protected meter communication or inspecting the security settings referenced by an association.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

<!-- versions: 0 -->
### 2. Security policy (version 0)

Selects no protection, authentication of all messages, encryption of all messages, or both authentication and encryption.
<!-- /versions -->

<!-- versions: 1 -->
### 2. Security policy (version 1)

Defines separate authentication, encryption and digital signature requirements for requests and responses using a set of policy flags.
<!-- /versions -->

<!-- versions: 0 -->
### 3. Security suite (version 0)

Specifies suite 0: AES-GCM-128 authenticated encryption and AES-128 key wrapping.
<!-- /versions -->

<!-- versions: 1 -->
### 3. Security suite (version 1)

Selects suite 0, 1 or 2. Suites 1 and 2 support digital signatures and key agreement in addition to authenticated encryption, with the algorithms and key sizes defined by the selected suite.
<!-- /versions -->

### 4. Client system title

Identifies the client system in protected communication. This identifier is distinct from its network address.

### 5. Server system title

Identifies the meter's server system in protected communication.

### 6. Certificates

Lists certificate information associated with public key security, including certificate role and identity details.
