create table "TokenTable"
(
    u_id    integer
        constraint tokentable_usertable_u_id_fk
            references "UserTable",
    t_token text,
    t_id    integer default nextval('tokentable_t_id_seq'::regclass) not null
        constraint tokentable_pk
            primary key
);

alter table "TokenTable"
    owner to postgres;

create unique index tokentable_t_id_uindex
    on "TokenTable" (t_id);

