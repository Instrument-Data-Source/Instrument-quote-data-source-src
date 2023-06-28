# ADR-004 Declare Event ID

## Context
- Text of errors is not crear definition of error, for service users will be more easy to use code to define error
- Logging is better when you log event ID

## Solution
### Single pool of EventId
- One place to store errors
- Hard to find exist errors

### Split pool of errors to each class and common pool
- Each class define errors which it use
- App has list of common errors which can be reused by another classes
- Need to have doc whene store all places where errors is stored

## Decision
In progress
