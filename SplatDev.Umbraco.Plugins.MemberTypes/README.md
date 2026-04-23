# UmbracoCms.Plugins.MemberTypes

Member type management plugin for Umbraco CMS. Create, edit, and manage custom member types with profile fields, custom properties, and type templates.

## Targets

- **Umbraco 13** (net8.0)
- **Umbraco 17** (net10.0)

## Features

- List all member types with property counts
- View member type details and properties
- Create new member types via API
- Update existing member types
- Delete member types
- Backoffice dashboard (AngularJS for U13, Lit 3 Web Component for U17)

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/umbraco/api/membertypes/GetAll` | List all member types |
| GET | `/umbraco/api/membertypes/GetByAlias?alias=...` | Get member type by alias |
| POST | `/umbraco/api/membertypes/Create` | Create a member type |
| PUT | `/umbraco/api/membertypes/Update?alias=...` | Update a member type |
| DELETE | `/umbraco/api/membertypes/Delete?alias=...` | Delete a member type |

## Client Build

```bash
cd client
npm install
npm run build
```
