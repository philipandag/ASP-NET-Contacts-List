interface Category {
    id: number
    name: string
    wildcardCategory: boolean
}

interface SubCategory {
    id: number
    name: string
    subcategoryForId: number
}

interface Contact {
    id: number
    name: string
    surname: string
    email: string
    phoneNumber: string
    mainCategory: Category
    subcategory: SubCategory
    dateOfBirth: string
}

var loggedIn: boolean = false;
var loggedInUser: Contact;

var siteurl: string = "https://localhost:7020";

// Tries to log in with the credentials from the form and handles the response
function Login() {
    // Get the email and password from the form
    let email: string = (<HTMLInputElement>document.getElementById("login-email")).value;
    let password: string = (<HTMLInputElement>document.getElementById("login-password")).value;

    // Send the login request and handle the response   
    let data = JSON.stringify({ email: email, password: password })
    var result = fetch(siteurl + "/api/Contacts/login", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: data
    })
    .then(data => {
        if (data.status == 200) {
            return data.json();
        }
        else if (data.status == 401) {
            showMessage("Wrong credentials");
            throw new Error("Wrong credentials");
        }
    })
    .then(data => {
        console.log('Login Success:', data);
        showMessage("Login successful");
        onLogin(data);
    })
    .catch((error) => {
        console.error('Login Error:', error);
        onLogout();
    })
}


// Tries to log out and handles the response
function Logout()
{
    // Send the logout request and handle the response
    var result = fetch(siteurl + "/api/Contacts/logout", {
        method: 'POST',
    })
    .then(data => {
        if (data.status == 200) {
            console.log('Logout Success:', data);
            showMessage("Logout successful");
            onLogout();
        }
        else {
            showMessage("Logout failed");
            console.error("Logout failed:", data);
        }

    })
    .catch((error) => {
        console.error('Logout Error:', error);
        onLogout();
    })
}

// Gets all contacts from the server
async function getContacts(): Promise<Contact[]> {
    console.log("Getting contacts");
    const response = await fetch(siteurl + "/api/Contacts");
    const data = await response.json();
    return data;
}

// Does what needs to be done when someone logs in
function onLogin(user: Contact) {

    loggedIn = true;
    loggedInUser = user;
    // Welcome message
    document.getElementById("logged-in-username").innerHTML = user.name + " " + user.surname;

    // Show the elements that should be visible only when logged in
    var onloggedInOnlyElements: NodeListOf<HTMLElement> = document.querySelectorAll(".LoggedInOnly");
    onloggedInOnlyElements.forEach(element => {
        showElement(element);
    });

    // Hide the elements that should be visible only when logged out
    var onloggedInOnlyElements: NodeListOf<HTMLElement> = document.querySelectorAll(".LoggedOutOnly");
    onloggedInOnlyElements.forEach(element => {
        hideElement(element);
    });

}

// Does what needs to be done when someone logs out
function onLogout() {
    loggedIn = false;
    loggedInUser = null;
    document.getElementById("logged-in-username").innerHTML = "";

    // Hide the elements that should be visible only when logged in
    var onloggedInOnlyElements: NodeListOf<HTMLElement> = document.querySelectorAll(".LoggedInOnly");
    onloggedInOnlyElements.forEach(element => {
        hideElement(element);

    });

    // Show the elements that should be visible only when logged out
    var onloggedInOnlyElements: NodeListOf<HTMLElement> = document.querySelectorAll(".LoggedOutOnly");
    onloggedInOnlyElements.forEach(element => {
        showElement(element);
    });


}

// Reloads the contents of the contacts list
function ReloadContacts() {
    console.log("Loading contacts")
    var list = document.getElementById("ContactsList");
    list.innerHTML = "";

    // Gets the contacts from the server and adds them to the list
    getContacts().then(contacts => {
        contacts.forEach(contact => {
            console.log(contact);
            var item = document.createElement("li");
            item.className = "Contact-Item";
            item.id = "Contact-Item-" + contact.id;

            var datasection = document.createElement("div");
            datasection.className = "Contact-Data-Section";

            var name = document.createElement("span");
            name.className = "Contact-Name Contact-Item-Data";
            name.innerHTML = contact.name;
            datasection.appendChild(name);

            var surname = document.createElement("span");
            surname.className = "Contact-Surname Contact-Item-Data";
            surname.innerHTML = contact.surname;
            datasection.appendChild(surname);

            var mainCategory = document.createElement("span");
            mainCategory.className = "Contact-MainCategory Contact-Item-Data";
            mainCategory.innerHTML = contact.mainCategory.name;
            datasection.appendChild(mainCategory);

            var subcategory = document.createElement("span");
            subcategory.className = "Contact-Subcategory Contact-Item-Data";
            subcategory.innerHTML = contact.subcategory.name;
            datasection.appendChild(subcategory);

            var optionsButton = document.createElement("button");
            optionsButton.className = "btn btn-primary Contact-Options-Button LoggedInOnly Appearing";
            if (!loggedIn) {
                optionsButton.classList.add("hidden");
            } else {
                optionsButton.classList.add("visible")
            }

            // button for showing the actions that can be done with the contact
            optionsButton.innerHTML = "...";
            optionsButton.onclick = () => ToggleShowContactOptions(contact.id);
            datasection.appendChild(optionsButton);

            // field for elements used to interact with the contact
            var optionsField = document.createElement("div");
            optionsField.className = "Contact-Options-Field Appearing hidden";
            optionsField.id = "Contact-Options-Field-" + contact.id;

            item.appendChild(datasection);
            item.appendChild(optionsField);


            list.appendChild(item);
        })
    })

    showMessage("Contacts reloaded");
}

// Shows or hides the options menu for a contact entry in contact list
function ToggleShowContactOptions(contactId: number) {

    console.log("Options for contact " + contactId)

    var options = <HTMLElement>document.getElementById("Contact-Options-Field-" + contactId);
    options.innerHTML = "";

    // Decide whether to show or hide the options
    if (elementVisible(options) || options.innerHTML != "") {
        hideElement(options)
    }
    else if(!elementVisible(options) || options.innerHTML == "") {
        showElement(options)

        var editButton = document.createElement("button");
        editButton.classList.add("btn", "btn-primary");
        editButton.onclick = () => EditContact(contactId);
        editButton.innerText = "Edit";
        options.appendChild(editButton);

        var deleteButton = document.createElement("button");
        deleteButton.onclick = () => DeleteContact(contactId);
        deleteButton.classList.add("btn", "btn-danger");
        deleteButton.innerText = "Delete";
        options.appendChild(deleteButton);            ;
    }

}

// Gets a contact from the server by id
async function GetContact(contactId: number): Promise<Contact> {
    console.log("Getting contact" + contactId);
    const response = await fetch(siteurl + "/api/Contacts/" + contactId);
    const data = await response.json();
    return data;
}

// Create form for editing a contact in contact list entry of given id
function EditContact(contactId: number) {
    console.log("Editing contact " + contactId);
    var options = document.getElementById("Contact-Options-Field-" + contactId);

    // First get the contact details from the server to be able to modify them
    GetContact(contactId).then(contact => {

        // Creating the form for editing the contact
        options.innerHTML = "";

        var form = document.createElement("form");
        form.className = "Visible Appearing edit-form";

        var nameLabel = document.createElement("label");
        nameLabel.innerHTML = "Name";
        nameLabel.htmlFor = "NameInput";
        form.appendChild(nameLabel);
        var nameInput = document.createElement("input");
        nameInput.type = "text";
        nameInput.placeholder = "Name";
        nameInput.name = "NameInput";
        nameInput.id = "NameInput";
        nameInput.value = contact.name;
        form.appendChild(nameInput);

        var surnameLabel = document.createElement("label");
        surnameLabel.innerHTML = "Surname";
        surnameLabel.htmlFor = "SurnameInput";
        form.appendChild(surnameLabel);
        var surnameInput = document.createElement("input");
        surnameInput.type = "text";
        surnameInput.placeholder = "Surname";
        surnameInput.name = "SurnameInput";
        surnameInput.id = "SurnameInput";
        surnameInput.value = contact.surname;
        form.appendChild(surnameInput);

        var emailLabel = document.createElement("label");
        emailLabel.innerHTML = "Email";
        emailLabel.htmlFor = "EmailInput";
        form.appendChild(emailLabel);
        var emailInput = document.createElement("input");
        emailInput.type = "text";
        emailInput.placeholder = "Email";
        emailInput.name = "EmailInput";
        emailInput.id = "EmailInput";
        emailInput.value = contact.email;
        form.appendChild(emailInput);

        var passwordLabel = document.createElement("label");
        passwordLabel.innerHTML = "Password";
        passwordLabel.htmlFor = "PasswordInput";
        form.appendChild(passwordLabel);
        var passwordInput = document.createElement("input");
        passwordInput.type = "password";
        passwordInput.placeholder = "Password";
        passwordInput.name = "PasswordInput";
        passwordInput.id = "PasswordInput";
        passwordInput.value = "";
        form.appendChild(passwordInput);

        var mainCategoryLabel = document.createElement("label");
        mainCategoryLabel.innerHTML = "Main Category";
        mainCategoryLabel.htmlFor = "MainCategoryInput";
        form.appendChild(mainCategoryLabel);
        var mainCategoryInput = document.createElement("select");
        mainCategoryInput.name = "MainCategoryInput";
        mainCategoryInput.id = "MainCategoryInput";
        form.appendChild(mainCategoryInput);

        // Get possible main categories
        GetCategoriesList().then(categories => {
            categories.forEach(category => {
                var option = document.createElement("option");
                option.value = JSON.stringify(category)
                if (category.id == contact.mainCategory.id) {
                    option.selected = true;
                }
                option.innerHTML = category.name;
                mainCategoryInput.appendChild(option);
            })
        })

        
        var subcategoryLabel = document.createElement("label");
        subcategoryLabel.innerHTML = "Subcategory";
        subcategoryLabel.htmlFor = "SubcategoryInput";
        form.appendChild(subcategoryLabel);
        var subcategoryInput = document.createElement("select");
        subcategoryInput.name = "SubcategoryInput";
        subcategoryInput.id = "SubcategoryInput";

        // Get possible subcategories for the main category
        var updateSubdirectories = () => {
            GetSubcategoriesList(contact.mainCategory.id).then(subcategories => {
                subcategories.forEach(subcategory => {
                    var option = document.createElement("option");
                    option.value = JSON.stringify(subcategory)
                    if (subcategory.id == contact.subcategory.id) {
                        option.selected = true;
                    }

                    option.innerHTML = subcategory.name;
                    subcategoryInput.appendChild(option);
                })
            })
        }


        updateSubdirectories();
        form.appendChild(subcategoryInput);

        // every time the main category changes, update the possible subcategories list
        mainCategoryInput.onchange = () => {
            updateSubdirectories()
        }


        var phoneLabel = document.createElement("label");
        phoneLabel.innerHTML = "Phone Number";
        phoneLabel.htmlFor = "PhoneInput";
        form.appendChild(phoneLabel);
        var phoneInput = document.createElement("input");
        phoneInput.type = "text";
        phoneInput.placeholder = "Phone Number";
        phoneInput.name = "PhoneInput";
        phoneInput.id = "PhoneInput";
        phoneInput.value = contact.phoneNumber;
        form.appendChild(phoneInput);

        var dateofbirthLabel = document.createElement("label");
        dateofbirthLabel.innerHTML = "Date of Birth";
        dateofbirthLabel.htmlFor = "DateOfBirthInput";
        form.appendChild(dateofbirthLabel);
        var dateofbirthInput = document.createElement("input");
        dateofbirthInput.type = "date";
        dateofbirthInput.placeholder = "Date of Birth";
        dateofbirthInput.name = "DateOfBirthInput";
        dateofbirthInput.id = "DateOfBirthInput";
        dateofbirthInput.value = contact.dateOfBirth.split("T")[0]; // date is in format yyyy-mm-ddThh:mm:ss - cut the time part
        form.appendChild(dateofbirthInput);

        options.appendChild(form);

        var saveButton = document.createElement("button");
        saveButton.innerHTML = "Save";
        saveButton.classList.add("btn", "btn-danger");
        saveButton.onclick = () => {
            var updatedContact = {
                id: contact.id,
                name: nameInput.value,
                surname: surnameInput.value,
                email: emailInput.value,
                password: passwordInput.value,
                mainCategory: JSON.parse(mainCategoryInput.value),
                subcategory: JSON.parse(subcategoryInput.value),
                phoneNumber: phoneInput.value,
                dateOfBirth: dateofbirthInput.value
            }

            // send a put request to update the contact and reload the contacts list
            fetch(siteurl + "/api/Contacts/" + contact.id, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(updatedContact)
            }).then((data) => {
                console.log(data);
                ReloadContacts();
                showMessage("Contact updated successfully");
            })
        }
        options.appendChild(saveButton);

        // add a cancel button to cancel the editing
        var cancelButton = document.createElement("button");
        cancelButton.innerHTML = "Cancel";
        cancelButton.classList.add("btn", "btn-primary");
        cancelButton.onclick = () => {
            options.innerHTML = "";
            hideElement(options);
        }
        options.appendChild(cancelButton);


    })

    
}

// Creates a form for deleting selected contact
function DeleteContact(contactId: number) {
    console.log("Deleting contact number" + contactId)
    var options = document.getElementById("Contact-Options-Field-" + contactId);
    options.innerHTML = "";

    var form = document.createElement("form");
    form.className = "form-group";

    var label = document.createElement("label");
    label.innerHTML = "Are you sure you want to delete this contact?";
    var yesbutton = document.createElement("button");
    yesbutton.innerHTML = "Yes";
    yesbutton.classList.add("btn", "btn-danger");
    yesbutton.onclick = () => {
        fetch("https://localhost:7020/api/Contacts/" + contactId, {
            method: "DELETE"
        }).then((data) => {
            console.log("Deleted", data)
            ReloadContacts();
            showMessage("Contact deleted successfully");
        })
    }

    var nobutton = document.createElement("button");
    nobutton.innerHTML = "No";
    nobutton.classList.add("btn", "btn-primary");
    nobutton.onclick = () => {
        options.innerHTML = "";
        hideElement(options);
    }

    form.appendChild(label);
    form.appendChild(nobutton);
    form.appendChild(yesbutton);

    options.appendChild(form);

}

// Hides the options for the selected contact
function HideContactOptions(contactId: number) {
    console.log("Hiding options for contact " + contactId)
    var item = document.getElementById("Contact-Options-Field-" + contactId);
    item.innerHTML = "";
}

// Shows or hides the create contact form
function ToggleShowCreateContact() {
    var createContactForm = document.getElementById("create-contact-form");

    if (elementVisible(createContactForm)) {
        hideElement(createContactForm);
        document.getElementById("show-create-contact-form-button").innerHTML = "+";
    }
    else {
        showElement(createContactForm);
        document.getElementById("show-create-contact-form-button").innerHTML = "-";
        UpdateCategoriesList();
    }
}

// adds a contact to the database using the data from the create contact form
function CreateContact() {
    console.log("Adding contact")

    var name = (<HTMLInputElement>document.getElementById("create-contact-name")).value;
    var surname = (<HTMLInputElement>document.getElementById("create-contact-surname")).value;

    var email = (<HTMLInputElement>document.getElementById("create-contact-email")).value;
    var password = (<HTMLInputElement>document.getElementById("create-contact-password")).value;
    var phoneNumber = (<HTMLInputElement>document.getElementById("create-contact-phone")).value;
    var dateOfBirth = new Date((<HTMLInputElement>document.getElementById("create-contact-dateofbirth")).value);

    var cat: Category = JSON.parse((<HTMLInputElement>document.getElementById("create-contact-category")).value);

    var subcatvalue = (<HTMLInputElement>document.getElementById("create-contact-subcategory")).value;
    var subcat: SubCategory;
    if (subcatvalue == "") {
        var subcatname = (<HTMLInputElement>document.getElementById("create-custom-contact-subcategory")).value;
        subcat = { name: subcatname, id: 0, subcategoryForId: cat.id };
    }
    else {
        subcat = JSON.parse((<HTMLInputElement>document.getElementById("create-contact-subcategory")).value);
    }
    

    var contact = {name:name, surname:surname, email:email, password: password, phoneNumber:phoneNumber, dateOfBirth:dateOfBirth, mainCategory:cat, subcategory:subcat}
    let data = JSON.stringify(contact)

    // send the POST request to create a new contact from data from the form
    var result = fetch("https://localhost:7020/api/Contacts", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: data
    })
    .then(data => data.json())
    .then(data => {
        console.log('Create Contact Success:', data);

        document.getElementById("result-message").innerHTML = "Contact added successfully";
        ToggleShowCreateContact();
    })
    .catch((error) => {
        console.error('Create Contact Error:', error);
        document.getElementById("result-message").innerHTML = "Failed to add Contact";
        ToggleShowCreateContact();
    })

    ReloadContacts()
    showMessage("Contact added successfully");
}

// returns a list of all the categories
async function GetCategoriesList() {
    console.log("Getting contacts");
    const response = await fetch("https://localhost:7020/api/Categories");
    const data = await response.json();
    return data;
}


// updates the list of categories in the create contact form
function UpdateCategoriesList() {
    var categorySelect = <HTMLInputElement>document.getElementById("create-contact-category");
    categorySelect.innerHTML = "";
    var defaultoption = document.createElement("option");
    defaultoption.selected = true;
    defaultoption.disabled = true;
    defaultoption.innerHTML = "";
    categorySelect.appendChild(defaultoption);

    GetCategoriesList().then(categories => {
        categories.forEach(category => {
            var option = document.createElement("option");
            option.value = JSON.stringify(category);
            option.innerHTML = category.name;
            categorySelect.appendChild(option);
        })
    })
    UpdateSubcategoriesList();

}

// returns a list of all the subcategories of a category
async function GetSubcategoriesList(categoryId: number) {
    console.log("Getting contacts");
    const response = await fetch("https://localhost:7020/api/Categories/" + categoryId + "/Subcategories");
    const data = await response.json();
    return data;
}

// updates the list of subcategories in the create contact form
function UpdateSubcategoriesList() {
    var category = (<HTMLInputElement>document.getElementById("create-contact-category"));
    if (category.value == "") return;

    var selectedCategory = <Category>JSON.parse(category.value);
    if (selectedCategory.wildcardCategory) {
        showElement(document.getElementById("create-custom-contact-subcategory"));
        showElement(document.getElementById("create-custom-contact-subcategory-label"));
        hideElement(document.getElementById("create-contact-subcategory"))
        hideElement(document.getElementById("create-contact-subcategory-label"))
    } else {
        hideElement(document.getElementById("create-custom-contact-subcategory"));
        hideElement(document.getElementById("create-custom-contact-subcategory-label"));
        showElement(document.getElementById("create-contact-subcategory"))
        showElement(document.getElementById("create-contact-subcategory-label"))
    }

    var subcategorySelect = <HTMLInputElement>document.getElementById("create-contact-subcategory");
    subcategorySelect.innerHTML = "";
    var defaultoption = document.createElement("option");
    defaultoption.selected = true;
    defaultoption.disabled = true;
    defaultoption.innerHTML = "";
    subcategorySelect.appendChild(defaultoption);

    GetSubcategoriesList(selectedCategory.id).then(subcategories => {
        subcategories.forEach(category => {
            var option = document.createElement("option");
            option.value = JSON.stringify(category);;
            option.innerHTML = category.name;
            subcategorySelect.appendChild(option);
        })
    })

}

// Shows an element
function showElement(element: HTMLElement) {
    element.classList.remove("hidden");
    element.classList.add("visible");
}

// Hides an element
function hideElement(element: HTMLElement) {
    element.classList.remove("visible");
    element.classList.add("hidden");
}

// Checks if an element is visible or hidden in terms of showElement and hideElement functions
function elementVisible(element: HTMLElement) {

    if (element.classList.contains("hidden")) {
        return false;
    }
    return true;
}

// Shows a message in the result-message div
function showMessage(message: string) {
    document.getElementById("result-message").innerHTML = message;
}


window.onload = ReloadContacts;

