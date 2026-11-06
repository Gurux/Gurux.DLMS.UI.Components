# G3-PLC MAC setup

<!-- cosem-attributes: 22,22,25,26 -->

Configures addressing, security and access to the shared power line channel for a G3 modem. Neighbour information helps the modem choose suitable transmission settings.

## When to use it

For example, check the network identifier and neighbour table while commissioning a meter on a G3 network.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. MacShortAddress

Contains the short address used by this device on the local network.

### 3. MacRcCoord

Shows the route cost to the coordinator for use in beacon announcements.

### 4. MacPANId

Identifies the personal area network to which the device belongs.

### 5. MacKeyTable

Stores indexed encryption keys used to protect link layer frames.

### 6. MacFrameCounter

Tracks the security frame number used to distinguish new protected frames from replayed frames.

### 7. MacToneMask

Selects which carrier frequencies the modem may use for communication.

### 8. MacTmrTtl

Sets how long learned tone map settings remain usable before they need refreshing, in minutes.

### 9. MacMaxFrameRetries

Limits how often a frame is sent again after an unsuccessful transmission.

<!-- versions: 0-2 -->
### 10. MacNeighbourTableEntryTtl (versions 0-2)

Sets how long a neighbour table entry remains valid, in minutes.
<!-- /versions -->

<!-- versions: 3 -->
### 10. MacPosTableEntryTtl (version 3)

Sets how long direct-neighbour reachability entries in the POS table remain valid, in minutes.
<!-- /versions -->

### 11. MacNeighbourTable

Lists nearby devices and learned settings such as usable tones, modulation, gain and link quality.

### 12. MacHighPriorityWindowSize

Sets the number of channel access slots reserved for high priority traffic.

### 13. MacCscmFairnessLimit

Sets the number of failed backoffs used by the fairness mechanism before resetting its waiting range.

### 14. MacBeaconRandomizationWindowLength

Sets the time range over which beacon transmission is randomly delayed to reduce collisions.

### 15. MacA

Controls how quickly the adaptive contention window is reduced after channel access conditions improve.

### 16. MacK

Adjusts the rate at which the channel access fairness limit adapts.

### 17. MacMinCwAttempts

Sets the number of consecutive transmission attempts made with the minimum contention window.

### 18. MacCenelecLegacyMode

Reports whether the modem uses older or newer interleaving behaviour in the CENELEC frequency band.

### 19. MacFCCLegacyMode

Reports whether the modem uses older or newer modulation and interleaving behaviour in the FCC frequency band.

### 20. MacMaxBe

Sets the upper limit of the exponent that determines the random channel access waiting range.

### 21. MacMaxCsmaBackoffs

Limits the number of times channel access may be deferred before an attempt fails.

### 22. MacMinBe

Sets the lower limit of the exponent that determines the random channel access waiting range.

### 23. MacBroadcastMaxCwEnabled

Controls whether broadcast transmissions use the maximum contention window to reduce collisions.

### 24. MacTransmitAtten

Reduces the modem output level by the configured attenuation in decibels.

### 25. MacPosTable

Lists devices within direct communication range, with information used to track their reachability.

### 26. MacDuplicateDetectionTtl

Sets how long received frame identifiers are retained to recognise and reject duplicate frames.

## Versions

The installed component exposes attributes 1-22 for versions 0 and 1, attributes 1-25 for version 2, and attributes 1-26 for version 3. Version 3 changes attribute 10 to the POS table entry lifetime.

The component uses the version 1 field meanings for version 0 as well. It does not implement the original version 0 layout with 11 attributes and its different addressing, security and timer definitions.
