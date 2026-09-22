# 01 --- Atomic Design

## Hierarchy

### Foundation

Colors, typography, spacing, radius, shadows, breakpoints, motion,
z-index, and icons.

### Atoms

Button, IconButton, Input, Checkbox, Radio, Switch, Badge, Avatar, Icon,
Label, Tooltip, Divider.

### Molecules

SearchBox, FormField, DatePicker wrapper, UserAvatar, StatusBadge,
Pagination, TableActions, FilterItem, NotificationItem.

### Organisms

Sidebar, Topbar, DataTable, FilterBar, Modal, Drawer, ApprovalCard,
ApprovalTable, FormSection, DashboardCards.

### Templates

AppLayout, ListPage, DetailPage, FormPage, DashboardPage, ApprovalPage,
ReportPage, MasterDataPage.

### Pages

Pages are application/demo compositions and are not core reusable atomic
layers. Examples belong in `Stecon.UI.Demo`.

## Rules

-   Prefer composition over copying markup.
-   Do not create an Atom that knows about a page.
-   Organisms expose data/actions through parameters and events; they do
    not fetch application data directly.
-   Templates control layout, not business logic.
-   Domain-specific behavior stays in the consuming application unless
    deliberately promoted into the design system.
