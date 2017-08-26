Vue.component('user-item', {
    props: ['user'],
    template: '<li @click="select" class="v-user-item">{{user.name}}</li>',
    data: function () {
        return {
            user: null
        }
    },
    methods: {
        select: function () {
            this.$emit('select-user', this.user)
        }
    }
})

Vue.component('user-list', function (resolve, reject) {
    $.post('/api/user/ServiceCompanyUserList', function (res) {
        if (res.error) {
            reject(res.error)
        } else {
            resolve({
                template: '<ul class="v-user-list" >\
                            <user-item  @select-user="select" v-for="item in userlist" :user="item">\
                        </user-item></ul>',
                data: function () {
                    return {
                        userlist: res
                    }
                },
                methods: {
                    select: function (user) {
                        this.$emit('select-user', user)
                    }
                }
            })
        }
    })
})




