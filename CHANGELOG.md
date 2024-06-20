# CHANGELOG

- v1.3.0
  - Supports provision of a `Context` for a verification flow
  - The value should be provided at Start and Confirm
  - This increases security by insisting on some session commonality, for example:
    - The same IP address implies Start and Confirm were actioned from the same location
    - The same browser fingerprint implies Start and Confirm were actioned from the same browser
    - The same session id implies Start and Confirm were actioned from the same browser session

- v1.2.0
  - Returns false when `StartConfirmation` fails to send an email
    - Previously only returned false for failures to generate
    - The two cases are deliberately *not* distinguished between
      - This is because changing behaviour based on this is a security risk

- v1.1.0
  - NetStandard library
  - Cleared some warnings
  - Slight documentation updates
  - Mermaid flow diagram
  - Better sample site text

- v1.0.1
  - Small documentation fixes
  - Include request models in README

- v1.0.0
  - Custom confirmation email template
  - Add Nuget package definition
  - Stub web page with links

- v0.2.2
  - Multiple active tokens per key
    - Support an upper limit

- v0.2.1
  - Update README file
  - Reduce visibility of some publics

- v0.2.0
  - Initial solution and standard repo files
  - Background info added to the README file
  - Sample site added (DotNet 7 MVC, unchanged)
    - Cookie-based authentication configured and activated
    - Dashboard route added to check protection is enforced
  - ITokenStore interface and Token model
  - In-memory token store and configuration
  - Mailer and configuration
  - Main Permission Server class
  - DI extention to register Permission Server
  - Include Permission Server in the sample site
