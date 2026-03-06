CREATE TABLE IF NOT EXISTS employers (
    id INTEGER PRIMARY KEY,
	fullname TEXT NOT NULL,
    biometric TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS equipment_kinds (
    id INTEGER PRIMARY KEY,
    kind TEXT NOT NULL
);

INSERT INTO equipment_kinds VALUES
(1, 'Aferidor DR'), (2, 'Amperímetro'), (3, 'Maquineta'),
(4, 'Sequencímetro'), (5, 'Smartphone');

CREATE TABLE IF NOT EXISTS equipment_status (
    id INTEGER PRIMARY KEY,
    status TEXT NOT NULL
);

INSERT INTO equipment_status VALUES
(1, 'OK'), (2, 'Danificado'), (3, 'Quebrado');

CREATE TABLE IF NOT EXISTS equipments (
    id INTEGER PRIMARY KEY,
    id_kind INTEGER NOT NULL,
    id_status INTEGER NOT NULL,
    kit TEXT NOT NULL,
    FOREIGN KEY (id_kind) REFERENCES equipment_kinds (id),
    FOREIGN KEY (id_status) REFERENCES equipment_status (id),
);

CREATE TABLE IF NOT EXISTS transaction_kinds (
    id INTEGER PRIMARY KEY,
    kind TEXT NOT NULL
);

INSERT INTO transaction_kinds VALUES
(0, 'Parado'), (1, 'Retirado'), (3, 'Devolvido'),
(4, 'Manutenção'), (5, 'Removido');

CREATE TABLE IF NOT EXISTS transactions (
    id INTEGER PRIMARY KEY,
    created_at TIMESTAMP NOT NULL,
    id_equipment INTEGER NOT NULL,
    id_kind INTEGER NOT NULL,
    from_employer INTEGER NOT NULL,
    to_employer INTEGER NOT NULL,
    note TEXT DEFAULT NULL,
    FOREIGN KEY (id_kind) REFERENCES transaction_kinds (id),
    FOREIGN KEY (id_equipment) REFERENCES equipments (id),
    FOREIGN KEY (from_employer) REFERENCES employers (id),
    FOREIGN KEY (to_employer) REFERENCES employers (id)
);

CREATE INDEX IF NOT EXISTS idx_transactions_equipment ON transactions (id_equipment, created_at DESC);

CREATE VIEW view_report AS
SELECT
    t.created_at,
    tk.kind AS transaction_kind,
    e.id AS id_equipment,
    ek.kind AS equipment_kind,
    e.kit AS equipment_kit,
    ep1.id AS id_employer_from,
    ep1.fullname AS name_employer_from,
    ep2.id AS id_employer_to,
    ep2.fullname AS name_employer_to,
    es.status AS equipment_status,
    t.note
FROM equipments AS e
LEFT JOIN transactions AS t
    ON e.id = t.id_equipment
    AND t.created_at = (
        SELECT MAX(tr.created_at)
        FROM transactions AS tr
        WHERE tr.id_equipment = e.id
    )
LEFT JOIN transaction_kinds AS tk
    ON tk.id = t.id_kind
LEFT JOIN equipment_kinds AS ek
    ON ek.id = e.id_kind
LEFT JOIN equipment_status AS es
    ON es.id = e.id_status
LEFT JOIN employers AS ep1
    ON t.from_employer = ep1.id
LEFT JOIN employers AS ep2
    ON t.to_employer = ep2.id;
