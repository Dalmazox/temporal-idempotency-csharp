create table public.accounts (
  id uuid not null primary key,
  owner varchar(255) not null,
  current_balance decimal(8,2) not null
);

create table public.transactions (
  id uuid not null primary key,
  account_id uuid not null references accounts (id),
  amount decimal(8,2) not null,
  creation_date timestamptz not null default current_timestamp
);

insert into public.accounts (id, owner, current_balance)
values (gen_random_uuid(), 'account1', 1000);