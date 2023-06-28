# ADR-005 In Model constructor use other entities

## Context
- When create model entity constructor, what should use Id of related entities or Entities

## Solution
### Id
- Simple to create constructor
- To validate that Id is valid, need to select entity from DB

### Other entities
- Harder to create constructor
- Simpler to validate that entity is valid, just check that entities.Id > 0. If it true that mean that entity exist in DB. Entity will controll it by itself

## Decision
Use other entities in constructor, because in both case we need select data from DB. But in second solution we will not has problem to pass repository into validor to validate that Id exist in DB.
