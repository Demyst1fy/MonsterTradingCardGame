create table "UserTable"
(
    u_id         integer default nextval('usertable_id_seq'::regclass) not null
        constraint usertable_pkey
            primary key,
    u_username   text                                                  not null,
    u_password   text                                                  not null,
    u_coins      integer default 20,
    u_fullname   text                                                  not null,
    u_bio        text    default 'I am a new player!'::text            not null,
    u_image      text    default ':)'::text,
    u_spentcoins integer default 0
);

alter table "UserTable"
    owner to postgres;

create unique index usertable_username_uindex
    on "UserTable" (u_username);

