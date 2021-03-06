import React from "react";
import AdminPanel from "../../components/Admin/AdminPanel";

export default function Admin() {
	return (
		<AdminPanel>
			<AdminPanel.Link to="/admin/create/user" heading="Create User" />
			<AdminPanel.Link
				to="/admin/create/charge"
				heading="Create Charge Account"
			/>
			<AdminPanel.Link
				to="/admin/create/credit"
				heading="Create Credit Account"
			/>
			<AdminPanel.Link
				to="/admin/create/deposit"
				heading="Create Deposit Account"
			/>
			<AdminPanel.Link to="/admin/create/wallet" heading="Create Wallet" />
			<AdminPanel.Link to="/admin/create/debit" heading="Create Debit Card" />

			<AdminPanel.Link to="/admin/support" heading="Support" />

			<AdminPanel.Link to="/admin/delete/user" heading="Delete User" red />
			<AdminPanel.Link
				to="/admin/delete/account"
				heading="Delete Account"
				red
			/>
		</AdminPanel>
	);
}
