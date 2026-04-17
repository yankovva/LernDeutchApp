# LerningApp

LerningApp is an ASP.NET Core MVC web application for learning German through structured courses, lessons, vocabulary cards, and interactive exercises. The platform supports different types of users and includes a teacher approval workflow, teacher content management, and an administrative panel for moderation and platform control.

## Project Overview

The idea behind the project is to provide a learning platform where regular users can register, browse courses, and improve their language skills, while teachers can create and manage educational content after being reviewed and approved by an administrator.

The application separates account identity, teacher-specific data, and role-based permissions in order to support a clear approval and moderation flow.

## Main Goals

- Provide a structured environment for learning German
- Allow users to enroll in courses and track progress
- Allow approved teachers to create and manage educational content
- Allow administrators to manage users, teachers, and platform operations
- Support moderation of teacher applications and profile changes
- Keep a clear distinction between user identity, teacher profile data, and authorization roles

## Tech Stack

- ASP.NET Core MVC
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- Razor Views
- Bootstrap
- Custom service layer with repositories
- Docker for local SQL Server setup on macOS

## User Roles

The application currently uses the following main roles:

- `User`
- `Teacher`
- `Admin`

### User

A regular user can:

- register and log in
- browse available courses
- enroll in courses
- view personal account information
- track learning progress

### Teacher

A teacher is not just a role. A teacher also has a dedicated `Teacher` entity that stores teacher-specific information such as:

- biography
- qualifications
- teacher status
- teacher approval state
- teacher profile review data

A user becomes a teacher only after:

1. a `Teacher` entity exists for that user
2. the teacher profile is completed and submitted for review
3. an administrator approves the request
4. the `Teacher` role is assigned

Approved teachers can access the teacher panel and manage educational content.

### Admin

An administrator can:

- manage users
- manage teacher requests
- review teacher profiles
- approve or reject teacher changes
- remove teacher access
- restore inactive teachers
- monitor key areas of the system

## Core Domain Design

### ApplicationUser

`ApplicationUser` is the identity and account model.

It stores general account-related data such as:

- email
- username
- first name
- last name
- phone number
- profile image
- authentication information
- deletion state

### Teacher

`Teacher` is a domain entity, separate from `ApplicationUser`.

It exists because teacher-specific information and workflows do not belong directly to the identity user model.

The `Teacher` entity stores:

- approval status
- teacher since date
- biography
- qualifications
- pending profile changes
- links to created courses, lessons, and exercises

### Important Distinction

A very important design decision in the project is the distinction between:

- `Teacher entity`
- `Teacher role`

These are not the same thing.

#### Teacher entity

This means the user is part of the teacher workflow.

A user may have a `Teacher` entity and still not be an approved teacher yet.

#### Teacher role

This means the user has already been approved and is allowed to access teacher-only functionality.

This distinction is important because:

- a user can start a teacher request without having teacher permissions yet
- only approved teachers should be able to access the teacher panel
- administrators must be able to review teacher requests before granting permissions

## Teacher Status Flow

The `Teacher` entity uses a status-based lifecycle.

### Draft

The user has a teacher record, but the profile is not yet ready for review.

### PendingReview

The teacher profile has been submitted and is waiting for admin review.

### Approved

The teacher has been approved and can receive or keep the `Teacher` role.

### Rejected

The teacher request or initial profile submission was reviewed and rejected.

### Inactive

The teacher was previously active, but the teacher role was removed and the profile is no longer active as a teacher profile.

## Teacher Approval Flow

The teacher flow is one of the most important parts of the project.

### Initial Teacher Request

1. An admin identifies a regular user who should enter the teacher process
2. A `Teacher` entity is created for that user with status `Draft`
3. The user completes the teacher profile
4. The user submits the profile for review
5. The status changes to `PendingReview`
6. The admin reviews the profile
7. If approved:
   - the status becomes `Approved`
   - the `Teacher` role is assigned
   - the user can access the teacher panel
8. If rejected:
   - the status becomes `Rejected`
   - the role is not assigned

### Teacher Profile Edits After Approval

Once a teacher is already approved, profile edits should still go through moderation.

The reason is that teacher profile information may be visible to users and should not be changed freely without review.

For that reason:

- approved teacher profile data remains in the live fields
- new edits are stored in separate pending fields
- the admin reviews the pending profile changes
- only approved changes are applied to the live teacher profile

## Pending Teacher Profile Changes

To support moderated profile editing, the project stores pending teacher profile data separately.

These pending fields are used for teacher profile review and may include:

- pending first name
- pending last name
- pending phone number
- pending profile image
- pending biography
- pending qualification

There is also a boolean flag that indicates whether an approved teacher currently has profile changes waiting for admin review.

This allows the application to support:

- first-time teacher approval
- later profile updates for already approved teachers

without exposing unapproved changes immediately.

## Why Teacher Profile Editing Is Outside the Teacher Area

A key architectural decision in the project is that the teacher profile edit flow should not live only inside the `Teacher` area.

Why?

Because users with:

- `Draft`
- `PendingReview`
- `Rejected`

status do not yet have the `Teacher` role.

If teacher profile editing existed only inside the teacher area, these users would not be able to access the form needed to complete or resubmit their profile.

For that reason:

- the `Teacher` area is intended for already approved teachers
- teacher profile editing and review submission live in the main profile flow
- access to the teacher profile edit form is based on the existence of a `Teacher` entity, not just the `Teacher` role

## Admin Panel

The admin panel is designed as a separate management area from the public site.

It includes management features for:

- users
- teachers
- reports
- future platform moderation features

### Teacher Management

The teacher management area is based on the `Teacher` entity, not only on users in the `Teacher` role.

This is important because administrators need to see all teacher-related states, including:

- `Draft`
- `PendingReview`
- `Approved`
- `Rejected`
- `Inactive`

If the admin panel used only the `Teacher` role, it would miss all unapproved teacher requests, which are exactly the cases that need review.

## Teacher Panel

The teacher panel is a dedicated workspace for approved teachers.

It is intended for:

- dashboard access
- course management
- lesson management
- future exercise and content management

Only users with the `Teacher` role should access this area.

## User Profile

The user profile page provides a personal account overview.

It may include:

- profile image
- first name
- last name
- email
- username
- phone number
- native language
- enrolled courses
- completed courses
- completed lessons
- learned words
- account-related summaries

The user profile and the teacher profile are related but not identical.

## Course and Lesson Ownership

The project includes ownership-based logic for educational content.

Teachers create content such as:

- courses
- lessons
- vocabulary cards
- exercises

In service-layer authorization checks, ownership is verified through the teacher identity or, when necessary, bypassed for administrators.

This allows:

- teachers to manage their own content
- admins to review or intervene where appropriate

## Admin Bypass Logic

The project includes an admin bypass in places where the application checks whether the current teacher owns a given resource.

This means administrators can still access and manage data that normally belongs to a specific teacher.

This is especially important in content management and moderation scenarios.

## Soft Delete and Teacher Removal Logic

When a teacher is no longer active, the teacher should not always be fully removed from the system.

There are different scenarios:

### Remove Teacher Role

Used for approved teachers.

This removes the `Teacher` role and sets the teacher status to `Inactive`, while keeping the teacher entity and approved profile data for future restoration if needed.

### Remove Pending Teacher Request

Used for unapproved teacher records.

This can fully remove the teacher request when the profile was never approved and should no longer remain in the system.

### Reject Teacher Request or Profile Changes

Rejecting is different from removing.

- rejecting the initial request keeps the teacher entity but sets status to `Rejected`
- rejecting profile changes for an already approved teacher clears pending changes and leaves the teacher approved

## Future Improvements

Planned or possible future improvements include:

- notifications for teacher approval and rejection
- admin rejection notes or moderation comments
- dedicated reports and content moderation tools
- more advanced teacher profile review history
- stronger documentation for application flows
- public teacher profile presentation
- improved student progress dashboards
- richer course and lesson authoring tools

## Running the Project Locally

Typical local setup includes:

1. clone the repository
2. restore dependencies
3. configure `appsettings`
4. run database migrations
5. make sure SQL Server is available
6. run the web application

Because the project is developed on macOS, SQL Server may be run through Docker instead of a direct local installation.

## Design Notes

Some important project decisions are:

- `ApplicationUser` and `Teacher` are intentionally separate
- the `Teacher` role is not assigned immediately when a teacher request starts
- teacher approval is status-based and moderated
- teacher profile changes are not applied directly before admin approval
- admin management is based on teacher records and statuses, not just roles
- approved teacher functionality is separated from the teacher request/profile-completion flow

## Current State

The project already includes:

- authentication and identity integration
- role seeding for admin and teacher
- admin panel
- teacher panel
- user profile page
- teacher profile page
- teacher profile edit and review flow
- course and lesson management foundations
- teacher request lifecycle
- admin approval logic for teacher activation and profile updates

## Notes

This project is still evolving and the architecture is being refined as new workflows and moderation needs become clearer. The main goal is to keep the platform realistic, maintainable, and expandable as the feature set grows.
