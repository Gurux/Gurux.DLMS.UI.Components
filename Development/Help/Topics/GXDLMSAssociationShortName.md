# Short name association

<!-- cosem-attributes: 2,2,4 -->

This object describes a client connection that accesses meter attributes through short addresses. It provides the address information needed to find objects in that connection.

## When to use it

Use it when connecting to a meter that uses short name referencing and building its object list.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Object list

Lists the objects visible to the client, with each object's base short name, class, version and logical name.

### 3. Access rights list

Describes which attributes can be read or written and which methods can be called.

### 4. Security setup reference

Identifies the Security setup object responsible for this association's communication protection.
