create table "StatsTable"
(
    s_wins     integer default 0,
    s_losses   integer default 0,
    s_draws    integer default 0,
    s_elo      integer default 100,
    u_username text                                                     not null
        constraint statstable_usertable_u_username_fk
            references "UserTable" (u_username),
    s_id       integer default nextval('statstable_s_id_seq'::regclass) not null
        constraint statstable_pk
            primary key
);

alter table "StatsTable"
    owner to postgres;

create unique index statstable_s_id_uindex
    on "StatsTable" (s_id);

