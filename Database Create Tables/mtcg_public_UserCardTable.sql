create table "UserCardTable"
(
    c_id       text             not null
        constraint cardtable_pk
            primary key,
    c_name     text             not null,
    c_damage   double precision not null,
    c_indeck   boolean,
    u_username text
        constraint usercardtable_usertable_u_username_fk
            references "UserTable" (u_username)
);

alter table "UserCardTable"
    owner to postgres;

create unique index cardtable_c_id_uindex
    on "UserCardTable" (c_id);

