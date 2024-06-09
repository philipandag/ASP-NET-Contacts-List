var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
function TSButton() {
    let name = "Fred";
    document.getElementById("TSButton").innerHTML = "Hello, " + name;
}
function Login() {
    let email = document.getElementById("email").value;
    let password = document.getElementById("password").value;
    let data = { Email: email, Password: password };
    var result = fetch("https://localhost:7020/api/Contacts/login", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
        .then(response => response.json())
        .then(data => {
        console.log('Login Success:', data);
    })
        .catch((error) => {
        console.error('Login Error:', error);
    });
}
function getContacts() {
    return __awaiter(this, void 0, void 0, function* () {
        console.log("Getting contacts");
        const response = yield fetch("https://localhost:7020/api/Contacts");
        const data = yield response.json();
        return data;
    });
}
function ContactOptions(id) {
    console.log("Options for contact " + id);
}
function LoadContacts() {
    console.log("Loading contacts");
    var list = document.getElementById("ContactsList");
    getContacts().then(contacts => {
        contacts.forEach(contact => {
            console.log(contact);
            var item = document.createElement("li");
            var left = document.createElement("div");
            left.className = "Contact-Item-Info";
            var right = document.createElement("div");
            right.className = "Contact-Options";
            item.className = "Contact-Item";
            var name = document.createElement("span");
            name.className = "Contact-Name";
            name.innerHTML = contact.name;
            left.appendChild(name);
            var surname = document.createElement("span");
            surname.className = "Contact-Surname";
            surname.innerHTML = contact.surname;
            left.appendChild(surname);
            var mainCategory = document.createElement("span");
            mainCategory.className = "Contact-MainCategory";
            mainCategory.innerHTML = contact.mainCategory.name;
            left.appendChild(mainCategory);
            var subCategory = document.createElement("span");
            mainCategory.className = "Contact-Subcategory";
            mainCategory.innerHTML = contact.subCategory.name;
            left.appendChild(subCategory);
            var optionsButton = document.createElement("button");
            optionsButton.className = "btn btn-primary btn-Contact-Options";
            optionsButton.innerHTML = "...";
            optionsButton.onclick = () => ContactOptions(contact.id);
            right.appendChild(optionsButton);
            item.appendChild(left);
            item.appendChild(right);
            list.appendChild(item);
        });
    });
}
window.onload = LoadContacts;
//# sourceMappingURL=app.js.map