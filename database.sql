CREATE TABLE IF NOT EXISTS employers (
    id INTEGER PRIMARY KEY,
	fullname TEXT NOT NULL,
    biometric TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS equipments (
    id INTEGER PRIMARY KEY,
    kind INTEGER NOT NULL,
    status INTEGER NOT NULL,
    kit TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS transactions (
    id INTEGER PRIMARY KEY,
    created_at TIMESTAMP NOT NULL,
    id_equipment INTEGER NOT NULL,
    kind INTEGER NOT NULL,
    from_employer INTEGER NOT NULL,
    to_employer INTEGER NOT NULL,
    note TEXT DEFAULT NULL,
    FOREIGN KEY (id_equipment) REFERENCES equipments (id)
	FOREIGN KEY (from_employer) REFERENCES employers (id),
	FOREIGN KEY (to_employer) REFERENCES employers (id)
);

CREATE INDEX IF NOT EXISTS idx_transactions_equipment ON transactions (id_equipment, created_at DESC);