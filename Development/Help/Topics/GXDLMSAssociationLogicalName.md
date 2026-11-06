# Logical name association

<!-- cosem-attributes: 8,9,11,11 -->

This object describes the connection between a client and a meter when objects are addressed by logical name. It defines which objects and operations the client can use, together with authentication and session settings.

## When to use it

Use it when discovering the objects available to a meter reading client or checking why that client cannot write an attribute.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

<!-- versions: 0-2 -->
### 2. Object list (versions 0-2)

Lists the objects visible through this association, including their class, version, logical name and attribute and method access rights.
<!-- /versions -->

<!-- versions: 3 -->
### 2. Object list (version 3)

Lists the objects visible through this association, including their class, version, logical name and access rights. Access rights specify permitted operations and their authentication, encryption and digital signature requirements.
<!-- /versions -->

### 3. Associated partners ID

Identifies the client and server application partners for this association.

### 4. Application context name

Identifies the application context, including logical name referencing and whether protected communication is used.

### 5. xDLMS context information

Describes supported services, message size limits and protocol settings used when establishing the session.

### 6. Authentication mechanism name

Identifies the authentication method required to establish this association.

### 7. Secret

Holds the password or shared secret used by the selected authentication method. Some methods manage their keys through Security setup instead.

### 8. Association status

Shows whether the association is disconnected, being established or established.

### 9. Security setup reference

Identifies the Security setup object that manages protection for this association.

### 10. User list

Lists the user identifiers and names allowed to use the association. An empty list allows any user identifier.

### 11. Current user

Identifies the user of the current association.
